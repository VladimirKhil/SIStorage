using LinqToDB;
using LinqToDB.AspNet;
using LinqToDB.AspNet.Logging;
using LinqToDB.Data.RetryPolicy;
using LinqToDB.DataProvider.PostgreSQL;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using SIStorage.Database.Migrations;
using SIStorage.Database.Models;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace SIStorage.Database;

/// <summary>
/// Provides a <see cref="IServiceCollection" /> extension that allows to register a SIStorage database.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds SIStorage database to the service collection.
    /// </summary>
    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicConstructors, typeof(NpgsqlProviderAdapter.NpgsqlConnection))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicMethods, typeof(NpgsqlDataReader))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(Initial))]
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
        var dataProvider = PostgreSQLTools.GetDataProvider(PostgreSQLVersion.v15);

        services.AddLinqToDBContext<SIStorageDbConnection>((provider, options) =>
            options
                .UseConnectionFactory(dataProvider, _ => dataSource.CreateConnection())
                .UseRetryPolicy(new TransientRetryPolicy())
                .UseDefaultLogging(provider));

        DatabaseExtensions.InitJsonConversion(RoundModelContext.Default.RoundModelArray);
        DatabaseExtensions.InitJsonConversion(DictionaryStringShortContext.Default.DictionaryStringInt16);
    }
}

[JsonSerializable(typeof(RoundModel[]))]
internal partial class RoundModelContext : JsonSerializerContext { }

[JsonSerializable(typeof(Dictionary<string, short>))]
internal partial class DictionaryStringShortContext : JsonSerializerContext { }
