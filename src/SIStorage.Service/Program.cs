using AspNetCoreRateLimit;
using AutoMapper;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Conventions;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using Serilog;
using SIStorage.Database;
using SIStorage.Service.Configuration;
using SIStorage.Service.Contract;
using SIStorage.Service.Contracts;
using SIStorage.Service.Helpers;
using SIStorage.Service.MapperProfiles;
using SIStorage.Service.Metrics;
using SIStorage.Service.Middlewares;
using SIStorage.Service.Services;
using System.Data.Common;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, lc) => lc
    .ReadFrom.Configuration(ctx.Configuration)
    .Filter.ByExcluding(logEvent =>
        logEvent.Exception is BadHttpRequestException || logEvent.Exception is OperationCanceledException));

ConfigureServices(builder.Services, builder.Configuration);

var app = builder.Build();

app.UseSerilogRequestLogging();

Configure(app);

app.Run();

static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
{
    services.Configure<SIStorageOptions>(configuration.GetSection(SIStorageOptions.ConfigurationSectionName));

    services.AddControllers();
    
    services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo { Title = "SIStorage service", Version = "v1" });
        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        options.IncludeXmlComments(xmlPath);
        var xmlFileContract = $"{Assembly.GetExecutingAssembly().GetName().Name}.Contract.xml";
        var xmlPathContract = Path.Combine(AppContext.BaseDirectory, xmlFileContract);
        options.IncludeXmlComments(xmlPathContract);
    });

    ConfigureAutoMapper(services);

    services.AddSIStorageDatabase(configuration);
    ConfigureMigrationRunner(services, configuration);

    services.AddScoped<IFacetsApi, FacetsService>();
    services.AddScoped<IExtendedPackagesApi, PackagesService>();
    services.AddSingleton<ITempPackagesService, TempPackagesService>();
    services.AddSingleton<IPackageIndexer, PackageIndexer>();

    services.AddHostedService<TempPackagesCleaner>();

    AddRateLimits(services, configuration);
    AddMetrics(services);
}

static void ConfigureAutoMapper(IServiceCollection services)
{
    var mapperConfiguration = new MapperConfiguration(cfg => cfg.AddProfile<SIStorageProfile>());
    var mapper = mapperConfiguration.CreateMapper();

    services.AddSingleton(mapper);
}

static void ConfigureMigrationRunner(IServiceCollection services, IConfiguration configuration)
{
    services.AddSingleton<IConventionSet>(new DefaultConventionSet(DbConstants.Schema, null));

    var dbConnectionString = configuration.GetConnectionString("SIStorage");

    services
        .AddFluentMigratorCore()
        .ConfigureRunner(migratorBuilder =>
            migratorBuilder
                .AddPostgres()
                .WithGlobalConnectionString(dbConnectionString)
                .ScanIn(typeof(DbConstants).Assembly).For.Migrations())
        .AddLogging(lb => lb.AddFluentMigratorConsole());
}

static void Configure(WebApplication app)
{
    var options = app.Services.GetRequiredService<IOptions<SIStorageOptions>>().Value;

    app.UseMiddleware<ErrorHandlingMiddleware>();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    if (options.ServeStaticFiles)
    {
        var contentPath = StringHelper.BuildRootedPath(options.ContentFolder);
        Directory.CreateDirectory(contentPath);

        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(contentPath),
            ServeUnknownFileTypes = true
        });
    }

    app.UseRouting();
    app.MapControllers();

    app.UseIpRateLimiting();

    CreateDatabase(app);
    ApplyMigrations(app);

    app.UseOpenTelemetryPrometheusScrapingEndpoint();
}

static void AddRateLimits(IServiceCollection services, IConfiguration configuration)
{
    services.Configure<IpRateLimitOptions>(configuration.GetSection("IpRateLimit"));

    services.AddMemoryCache();
    services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
    services.AddInMemoryRateLimiting();
}

static void AddMetrics(IServiceCollection services)
{
    var meters = new OtelMetrics();

    services.AddOpenTelemetry().WithMetrics(builder =>
        builder
            .ConfigureResource(rb => rb.AddService("SIStorage"))
            .AddMeter(meters.MeterName)
            .AddAspNetCoreInstrumentation()
            .AddRuntimeInstrumentation()
            .AddProcessInstrumentation()
            .AddPrometheusExporter());

    services.AddSingleton(meters);
}

static void CreateDatabase(WebApplication app)
{
    var dbConnectionString = app.Configuration.GetConnectionString("SIStorage");

    var connectionStringBuilder = new DbConnectionStringBuilder
    {
        ConnectionString = dbConnectionString
    };

    connectionStringBuilder["Database"] = "postgres";

    DatabaseExtensions.EnsureExists(connectionStringBuilder.ConnectionString!, DbConstants.DbName);
}

static void ApplyMigrations(WebApplication app)
{
    var scope = app.Services.CreateScope();
    var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();

    if (runner.HasMigrationsToApplyUp())
    {
        runner.MigrateUp();
    }
}