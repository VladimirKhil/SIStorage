using SIStorage.Service.Contract.Models;

namespace SIStorage.Service.Contract.Responses;

/// <summary>
/// Defines packages result page.
/// </summary>
public sealed class PackagesPage
{
    /// <summary>
    /// Defines an empty page.
    /// </summary>
    public static readonly PackagesPage Empty = new();

    /// <summary>
    /// Collection of returned packages.
    /// </summary>
    public Package[] Packages { get; set; } = Array.Empty<Package>();

    /// <summary>
    /// Total number of packages. Used for requesting the next page.
    /// </summary>
    public int Total { get; set; }
}
