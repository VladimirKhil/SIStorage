namespace SIStorage.Service.Contract.Models;

/// <summary>
/// Provides SIGame package information.
/// </summary>
public sealed record Package
{
    /// <summary>
    /// Package unique global identifier. Should be unique across all package storages.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Package human readable name.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Package difficulty (from 1 to 10).
    /// </summary>
    public short? Difficulty { get; set; }

    /// <summary>
    /// Package restrictions (age or region, for example).
    /// This is a set of claims that must be satisfied to use this package.
    /// Restriction names are not required to be unique.
    /// Restrictions with the same name are joined with OR logic.
    /// Restrictions with different names are joined with AND logic.
    /// </summary>
    public int[]? RestrictionIds { get; set; }

    /// <summary>
    /// Package publisher identifier.
    /// </summary>
    public int? PublisherId { get; set; }

    /// <summary>
    /// Package authors identifiers.
    /// </summary>
    public int[] AuthorIds { get; set; } = Array.Empty<int>();

    /// <summary>
    /// Package create date.
    /// </summary>
    public DateOnly? CreateDate { get; set; }

    /// <summary>
    /// Package tags identifiers.
    /// </summary>
    public int[] TagIds { get; set; } = Array.Empty<int>();

    /// <summary>
    /// Package language code.
    /// </summary>
    public int? LanguageId { get; set; }

    /// <summary>
    /// Package location.
    /// </summary>
    public Uri? ContentUri { get; set; }

    /// <summary>
    /// Package direct content location (this link usage would not increase download counter).
    /// </summary>
    public Uri? DirectContentUri { get; set; }

    /// <summary>
    /// Package logo location.
    /// </summary>
    public Uri? LogoUri { get; set; }

    /// <summary>
    /// Package file size in bytes.
    /// </summary>
    public long? Size { get; set; }

    /// <summary>
    /// Package rounds info.
    /// </summary>
    public Round[]? Rounds { get; set; }

    /// <summary>
    /// Package total question count.
    /// </summary>
    public short? QuestionCount { get; set; }

    /// <summary>
    /// Package content type statistic. Keys are content types; values are the counts of specified content types in package.
    /// </summary>
    public Dictionary<string, short>? ContentTypeStatistic { get; set; }

    /// <summary>
    /// Package download count.
    /// </summary>
    public int? DownloadCount { get; set; }

    /// <summary>
    /// Package rating (from 0 to 5).
    /// </summary>
    public float? Rating { get; set; }

    /// <summary>
    /// Storage-specific additional information.
    /// </summary>
    public string? ExtraInfo { get; set; }
}
