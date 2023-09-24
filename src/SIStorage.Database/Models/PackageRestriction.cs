using LinqToDB;
using LinqToDB.Mapping;

namespace SIStorage.Database.Models;

/// <summary>
/// Defines a package - restriction relation.
/// </summary>
[Table(Schema = DbConstants.Schema, Name = DbConstants.PackageRestrictions)]
public sealed class PackageRestriction
{
    /// <summary>
    /// Package unique global identifier.
    /// </summary>
    [PrimaryKey, Column(DataType = DataType.Guid), NotNull]
    public Guid PackageId { get; set; }

    /// <summary>
    /// Restriction identifier.
    /// </summary>
    [PrimaryKey, Column(DataType = DataType.Int32), NotNull]
    public int RestrictionId { get; set; }
}
