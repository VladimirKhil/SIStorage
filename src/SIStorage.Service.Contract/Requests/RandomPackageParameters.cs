namespace SIStorage.Service.Contract.Requests;

/// <summary>
/// Defines random package parameters.
/// </summary>
public sealed record RandomPackageParameters
{
    /// <summary>
    /// Package tags.
    /// </summary>
    public int[]? TagIds { get; set; }

    /// <summary>
    /// Package difficulty.
    /// </summary>
    public short? Difficulty { get; set; }

    /// <summary>
    /// Package restrictions.
    /// Restrictions with the same name are joined with OR logic.
    /// Restrictions with different names are joined with AND logic.
    /// </summary>
    public int[]? RestrictionIds { get; set; }

    /// <summary>
    /// Package language.
    /// </summary>
    public int? LanguageId { get; set; }

    /// <summary>
    /// Round count.
    /// </summary>
    public int RoundCount { get; set; } = 3;

    /// <summary>
    /// Table round theme count.
    /// </summary>
    public int TableThemeCount { get; set; } = 6;

    /// <summary>
    /// Theme list round theme count.
    /// </summary>
    public int ThemeListThemeCount { get; set; } = 7;

    /// <summary>
    /// Base question price.
    /// </summary>
    public int BaseQuestionPrice { get; set; } = 100;
}
