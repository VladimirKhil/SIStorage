namespace SIStorage.Service.Contract.Models;

/// <summary>
/// Contains package round information.
/// </summary>
/// <param name="Name">Round name.</param>
/// <param name="ThemeNames">Round themes names.</param>
public sealed record Round(string Name, string[] ThemeNames);
