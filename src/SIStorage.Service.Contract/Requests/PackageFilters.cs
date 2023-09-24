namespace SIStorage.Service.Contract.Requests;

/// <summary>
/// Provides packages filtering information.
/// </summary>
public sealed class PackageFilters
{
    /// <summary>
    /// Publisher filter.
    /// </summary>
    public int? PublisherId { get; set; }

    /// <summary>
    /// Author filter.
    /// </summary>
    public int? AuthorId { get; set; }

    /// <summary>
    /// Tags filter.
    /// </summary>
    public int[]? TagIds { get; set; }

    /// <summary>
    /// Difficulty filter.
    /// </summary>
    public NumericFilter<short>? Difficulty { get; set; }

    /// <summary>
    /// Restriction filter.
    /// Restrictions with the same name are joined with OR logic.
    /// Restrictions with different names are joined with AND logic.
    /// </summary>
    public int[]? RestrictionIds { get; set; }

    /// <summary>
    /// Language identifier.
    /// </summary>
    public int? LanguageId { get; set; }

    /// <summary>
    /// Text to find in any of the package fields.
    /// </summary>
    public string? SearchText { get; set; }
}
