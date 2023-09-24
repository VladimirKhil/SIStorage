namespace SIStorage.Service.Contract.Requests;

/// <summary>
/// Provides packages selection parameters.
/// </summary>
public sealed class PackageSelectionParameters
{
    /// <summary>
    /// Results sort mode.
    /// </summary>
    public PackageSortMode SortMode { get; set; } = PackageSortMode.Name;

    /// <summary>
    /// Results sort direction.
    /// </summary>
    public PackageSortDirection SortDirection { get; set; } = PackageSortDirection.Ascending;

    /// <summary>
    /// "From" pagination parameter.
    /// </summary>
    public int From { get; set; }

    /// <summary>
    /// "Count" pagination parameter.
    /// </summary>
    public int Count { get; set; } = 20;
}
