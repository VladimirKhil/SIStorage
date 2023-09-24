namespace SIStorage.Service.Contract.Requests;

/// <summary>
/// Defines a value compare mode.
/// </summary>
[Flags]
public enum CompareMode
{
    /// <summary>
    /// Greater than target value.
    /// </summary>
    GreaterThan = 1,

    /// <summary>
    /// Equal to target value.
    /// </summary>
    EqualTo = 2,

    /// <summary>
    /// Less than target value.
    /// </summary>
    LessThan = 4,
}
