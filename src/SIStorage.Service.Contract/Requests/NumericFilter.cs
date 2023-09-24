namespace SIStorage.Service.Contract.Requests;

/// <summary>
/// Provides an numeric value filter.
/// </summary>
public sealed class NumericFilter<T> where T : struct
{
    /// <summary>
    /// Value compare mode.
    /// </summary>
    public CompareMode CompareMode { get; set; } = CompareMode.GreaterThan | CompareMode.EqualTo;

    /// <summary>
    /// Value to compare.
    /// </summary>
    public T Value { get; set; }
}
