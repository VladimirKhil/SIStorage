namespace SIStorage.Database.Models;

/// <summary>
/// Contains package round information.
/// </summary>
/// <remarks>
/// This model is embedded into packages table.
/// </remarks>
public sealed class RoundModel
{
    /// <summary>
    /// Round name.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Round themes names.
    /// </summary>
    public string[]? ThemeNames { get; set; }
}
