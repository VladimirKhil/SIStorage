using LinqToDB;
using LinqToDB.Mapping;

namespace SIStorage.Database.Models;

/// <summary>
/// Defines a package - author relation.
/// </summary>
[Table(Schema = DbConstants.Schema, Name = DbConstants.PackageAuthors)]
public sealed class PackageAuthor
{
    /// <summary>
    /// Package unique global identifier.
    /// </summary>
    [PrimaryKey, Column(DataType = DataType.Guid), NotNull]
    public Guid PackageId { get; set; }

    /// <summary>
    /// Author identifier.
    /// </summary>
    [PrimaryKey, Column(DataType = DataType.Int32), NotNull]
    public int AuthorId { get; set; }
}
