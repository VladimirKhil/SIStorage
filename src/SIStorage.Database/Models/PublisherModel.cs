using LinqToDB;
using LinqToDB.Mapping;

namespace SIStorage.Database.Models;

/// <summary>
/// Provides a package publisher info.
/// </summary>
[Table(Schema = DbConstants.Schema, Name = DbConstants.Publishers)]
public sealed class PublisherModel
{
    /// <summary>
    /// Publisher identifier.
    /// </summary>
    [PrimaryKey, Column(DataType = DataType.Int32), NotNull, Identity]
    public int Id { get; set; }

    /// <summary>
    /// Publisher name.
    /// </summary>
    [Column(DataType = DataType.NVarChar), NotNull]
    public string? Name { get; set; }
}
