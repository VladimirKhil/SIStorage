using LinqToDB;
using LinqToDB.Mapping;

namespace SIStorage.Database.Models;

/// <summary>
/// Provides SIGame package model.
/// </summary>
[Table(Schema = DbConstants.Schema, Name = DbConstants.Packages)]
public sealed class PackageModel
{
    /// <summary>
    /// Package unique global identifier. Should be unique across all packages storages.
    /// </summary>
    [PrimaryKey, Column(DataType = DataType.Guid), NotNull]
    public Guid Id { get; set; }

    /// <summary>
    /// Package human readable name.
    /// </summary>
    [Column(DataType = DataType.NVarChar), NotNull]
    public string? Name { get; set; }

    /// <summary>
    /// Package difficulty (from 1 to 10).
    /// </summary>
    [Column(DataType = DataType.Int16), Nullable]
    public short? Difficulty { get; set; }

    /// <summary>
    /// Package publisher identifier.
    /// </summary>
    [Column(DataType = DataType.Int32), Nullable]
    public int? PublisherId { get; set; }

    /// <summary>
    /// Package create date.
    /// </summary>
    [Column(DataType = DataType.Date), Nullable]
    public DateOnly? CreateDate { get; set; }

    /// <summary>
    /// Package language identifier.
    /// </summary>
    [Column(DataType = DataType.Int32), NotNull]
    public int? LanguageId { get; set; }

    /// <summary>
    /// Package original file name.
    /// </summary>
    [Column(DataType = DataType.NVarChar), NotNull]
    public string? OriginalFileName { get; set; }

    /// <summary>
    /// Package file name.
    /// </summary>
    [Column(DataType = DataType.NVarChar), NotNull]
    public string? FileName { get; set; }

    /// <summary>
    /// Package logo location.
    /// </summary>
    [Column(DataType = DataType.NVarChar), Nullable]
    public string? LogoUri { get; set; }

    /// <summary>
    /// Can the package be downloaded.
    /// </summary>
    [Column(DataType = DataType.Boolean), NotNull]
    public bool Downloadable { get; set; }

    /// <summary>
    /// Package download count.
    /// </summary>
    [Column(DataType = DataType.Int32), NotNull]
    public int DownloadCount { get; set; }

    /// <summary>
    /// Package file size in bytes.
    /// </summary>
    [Column(DataType = DataType.Int64), NotNull]
    public long Size { get; set; }

    /// <summary>
    /// Package rounds info.
    /// </summary>
    [Column(DataType = DataType.Json), NotNull]
    public RoundModel[]? Rounds { get; set; }

    /// <summary>
    /// Package total question count.
    /// </summary>
    [Column(DataType = DataType.Int16), NotNull]
    public short? QuestionCount { get; set; }

    /// <summary>
    /// Package content type statistic. Keys are content types; values are the counts of specified content types in package.
    /// </summary>
    [Column(DataType = DataType.Json), NotNull]
    public Dictionary<string, short>? ContentTypeStatistic { get; set; }
}
