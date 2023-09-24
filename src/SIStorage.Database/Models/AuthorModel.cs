using LinqToDB;
using LinqToDB.Mapping;

namespace SIStorage.Database.Models;

/// <summary>
/// Provides a package author info.
/// </summary>
[Table(Schema = DbConstants.Schema, Name = DbConstants.Authors)]
public sealed class AuthorModel
{
    /// <summary>
    /// Author identifier.
    /// </summary>
    [PrimaryKey, Column(DataType = DataType.Int32), NotNull, Identity]
    public int Id { get; set; }

    /// <summary>
    /// Author name.
    /// </summary>
    [Column(DataType = DataType.NVarChar), NotNull]
    public string? Name { get; set; }
}
