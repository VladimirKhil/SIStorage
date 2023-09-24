using AutoMapper;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Conventions;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using Serilog;
using SIStorage.Database;
using SIStorage.Service.Configuration;
using SIStorage.Service.Contract;
using SIStorage.Service.Contracts;
using SIStorage.Service.MapperProfiles;
using SIStorage.Service.Metrics;
using SIStorage.Service.Services;
using System.Data.Common;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, lc) => lc.ReadFrom.Configuration(ctx.Configuration));

ConfigureServices(builder.Services, builder.Configuration);

var app = builder.Build();

app.UseSerilogRequestLogging();

Configure(app);

app.Run();

static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
{
    services.Configure<SIStorageOptions>(configuration.GetSection(SIStorageOptions.ConfigurationSectionName));

    services.AddControllers();

    ConfigureAutoMapper(services);

    services.AddSIStorageDatabase(configuration);
    ConfigureMigrationRunner(services, configuration);

    services.AddScoped<IFacetsApi, FacetsService>();
    services.AddScoped<IExtendedPackagesApi, PackagesService>();

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
    app.UseRouting();
    app.MapControllers();

    CreateDatabase(app);
    ApplyMigrations(app);

    app.UseOpenTelemetryPrometheusScrapingEndpoint();
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