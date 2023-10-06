using SIStorage.Database.Models;

namespace SIStorage.Service.Models;

/// <summary>
/// Contains SIGame package information.
/// </summary>
public sealed class PackageInfo
{
    /// <summary>
    /// Package human readable name.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Package difficulty (from 1 to 10).
    /// </summary>
    public short? Difficulty { get; set; }

    /// <summary>
    /// Package publisher.
    public string? Publisher { get; set; }

    /// <summary>
    /// Package create date.
    /// </summary>
    public DateOnly? CreateDate { get; set; }

    /// <summary>
    /// Package language.
    /// </summary>
    public string? Language { get; set; }

    /// <summary>
    /// Package rounds info.
    /// </summary>
    public RoundModel[]? Rounds { get; set; }

    /// <summary>
    /// Package total question count.
    /// </summary>
    public short? QuestionCount { get; set; }

    /// <summary>
    /// Package content type statistic. Keys are content types; values are the counts of specified content types in package.
    /// </summary>
    public Dictionary<string, short>? ContentTypeStatistic { get; set; }

    /// <summary>
    /// Package authors.
    /// </summary>
    public string[] Authors { get; set; } = Array.Empty<string>();

    /// <summary>
    /// Package tags.
    /// </summary>
    public string[] Tags { get; set; } = Array.Empty<string>();

    /// <summary>
    /// Package restriction.
    /// </summary>
    public string Restriction { get; set; } = "";
}
