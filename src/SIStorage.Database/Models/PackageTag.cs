using LinqToDB;
using LinqToDB.Mapping;

namespace SIStorage.Database.Models;

/// <summary>
/// Defines a package - tag relation.
/// </summary>
[Table(Schema = DbConstants.Schema, Name = DbConstants.PackageTags)]
public sealed class PackageTag
{
    /// <summary>
    /// Package unique global identifier.
    /// </summary>
    [PrimaryKey, Column(DataType = DataType.Guid), NotNull]
    public Guid PackageId { get; set; }

    /// <summary>
    /// Tag identifier.
    /// </summary>
    [PrimaryKey, Column(DataType = DataType.Int32), NotNull]
    public int TagId { get; set; }
}
