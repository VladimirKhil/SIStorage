using LinqToDB;
using LinqToDB.Mapping;

namespace SIStorage.Database.Models;

/// <summary>
/// Defines a package restriction model.
/// </summary>
[Table(Schema = DbConstants.Schema, Name = DbConstants.Restrictions)]
public sealed class RestrictionModel
{
    /// <summary>
    /// Restriction Id.
    /// </summary>
    [PrimaryKey, Column(DataType = DataType.Int32), NotNull, Identity]
    public int Id { get; set; }

    /// <summary>
    /// Restriction name.
    /// </summary>
    [Column(DataType = DataType.NVarChar), NotNull]
    public string? Name { get; set; }

    /// <summary>
    /// Restriction value.
    /// </summary>
    [Column(DataType = DataType.NVarChar), NotNull]
    public string? Value { get; set; }
}
