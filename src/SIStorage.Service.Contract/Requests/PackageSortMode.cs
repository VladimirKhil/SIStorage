namespace SIStorage.Service.Contract.Requests;

/// <summary>
/// Defines a packages sort mode.
/// </summary>
public enum PackageSortMode
{
    /// <summary>
    /// Sort by name.
    /// </summary>
    Name = 0,

    /// <summary>
    /// Sort by created date.
    /// </summary>
    CreatedDate = 1,

    /// <summary>
    /// Sort by download count.
    /// </summary>
    DownloadCount = 2,

    /// <summary>
    /// Sort by rating.
    /// </summary>
    Rating = 3,
}
