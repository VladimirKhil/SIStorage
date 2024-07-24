using LinqToDB;
using LinqToDB.AspNet;
using LinqToDB.AspNet.Logging;
using LinqToDB.Data.RetryPolicy;
using LinqToDB.DataProvider.PostgreSQL;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using SIStorage.Database.Models;

namespace SIStorage.Database;

/// <summary>
/// Provides a <see cref="IServiceCollection" /> extension that allows to register a SIStorage database.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds SIStorage database to the service collection.
    /// </summary>
    public static void AddSIStorageDatabase(
        this IServiceCollection services,
        IConfiguration configuration,
        string connectionStringName = "SIStorage")
    {
        var dbConnectionString = configuration.GetConnectionString(connectionStringName)
            ?? throw new InvalidOperationException("Database connection is undefined");

        var builder = new NpgsqlDataSourceBuilder(dbConnectionString);
        builder.EnableDynamicJson();
        
        var dataSource = builder.Build();
        var dataProvider = PostgreSQLTools.GetDataProvider(connectionString: dbConnectionString);

        services.AddLinqToDBContext<SIStorageDbConnection>((provider, options) =>
            options
                .UseConnectionFactory(dataProvider, _ => dataSource.CreateConnection())
                .UseRetryPolicy(new TransientRetryPolicy())
                .UseDefaultLogging(provider));

        DatabaseExtensions.InitJsonConversion<RoundModel[]>();
        DatabaseExtensions.InitJsonConversion<Dictionary<string, short>>();
    }
}
