namespace SIStorage.Database.Models;

/// <summary>
/// Contains package round information.
/// </summary>
/// <remarks>
/// This model is embedded into packages table.
/// </remarks>
/// <param name="Name">Round name.</param>
/// <param name="ThemeNames">Round themes names.</param>
public sealed record RoundModel(string Name, string[] ThemeNames);
