using FluentMigrator.Builders;
using FluentMigrator.Builders.Create.Table;
using LinqToDB;
using LinqToDB.Data;
using LinqToDB.Mapping;
using Npgsql;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace SIStorage.Database;

/// <summary>
/// Provides helper methods for working with database.
/// </summary>
public static class DatabaseExtensions
{
    /// <summary>
    /// Checks database existence and create database if it is not present.
    /// </summary>
    /// <param name="connectionString">SQL server connection string.</param>
    /// <param name="dbName">Database name.</param>
    /// <remarks>
    /// This method is intended to run on startup and thus is executed synchronously.
    /// </remarks>
    public static bool EnsureExists(string connectionString, string dbName)
    {
        using var dbConnection = new NpgsqlConnection(connectionString);

        dbConnection.Open();

        var existCmd = dbConnection.CreateCommand();
        existCmd.CommandText = "select count(*) from pg_database where datname = @name";
        existCmd.Parameters.Add(new NpgsqlParameter("name", dbName.ToLowerInvariant()));

        var existed = Convert.ToInt32(existCmd.ExecuteScalar());

        if (existed != 0)
        {
            return false;
        }

        var createCmd = dbConnection.CreateCommand();
        createCmd.CommandText = $"CREATE DATABASE \"{dbName.ToLowerInvariant()}\"";
        createCmd.ExecuteNonQuery();

        return true;
    }

    /// <summary>
    /// Registers a converter required to deserialize a JSON column value into object's property of specified type and serialize it back.
    /// </summary>
    /// <typeparam name="T">Object's property type.</typeparam>
    /// <exception cref="ArgumentException">Invalid value has been provided.</exception>
    public static void InitJsonConversion<T>(JsonTypeInfo<T> jsonTypeInfo) =>
        MappingSchema.Default
            .SetConverter<string, T>(
                value => JsonSerializer.Deserialize<T>(value, jsonTypeInfo)
                ?? throw new ArgumentException($"Invalid value {value} for deserialization to type {typeof(T)}"),
                LinqToDB.Common.ConversionType.FromDatabase)
            .SetConverter<T, DataParameter>(
                value =>
                    new DataParameter("", JsonSerializer.Serialize(value, jsonTypeInfo), DataType.Json)
                ?? throw new ArgumentException($"Invalid value {value} for serialization from type {typeof(T)}"),
                LinqToDB.Common.ConversionType.ToDatabase);

    internal static ICreateTableColumnOptionOrWithColumnSyntax AsJson(this IColumnTypeSyntax<ICreateTableColumnOptionOrWithColumnSyntax> builder) =>
        builder.AsCustom("json");
}
