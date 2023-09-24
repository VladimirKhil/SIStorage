using LinqToDB;
using LinqToDB.AspNet;
using LinqToDB.AspNet.Logging;
using LinqToDB.Data.RetryPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
        var dbConnectionString = configuration.GetConnectionString(connectionStringName);

        services.AddLinqToDBContext<SIStorageDbConnection>((provider, options) =>
            options
                .UsePostgreSQL(dbConnectionString)
                .UseRetryPolicy(new TransientRetryPolicy())
                .UseDefaultLogging(provider));

        DatabaseExtensions.InitJsonConversion<RoundModel[]>();
        DatabaseExtensions.InitJsonConversion<Dictionary<string, short>>();
    }
}
