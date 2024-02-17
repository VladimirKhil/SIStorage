using LinqToDB;
using LinqToDB.Mapping;

namespace SIStorage.Database.Models;

/// <summary>
/// Provides package tag info.
/// </summary>
[Table(Schema = DbConstants.Schema, Name = DbConstants.Tags)]
public sealed class TagModel
{
    /// <summary>
    /// Tag identifier.
    /// </summary>
    [PrimaryKey, Column(DataType = DataType.Int32), NotNull, Identity]
    public int Id { get; set; }

    /// <summary>
    /// Tag name.
    /// </summary>
    [Column(DataType = DataType.NVarChar), NotNull]
    public string Name { get; set; } = "";
}
