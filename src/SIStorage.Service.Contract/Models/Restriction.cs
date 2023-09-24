namespace SIStorage.Service.Contract.Models;

/// <summary>
/// Defines a package restriction.
/// </summary>
/// <param name="Id">Restriction identifier.</param>
/// <param name="Name">Restriction name.</param>
/// <param name="Value">Restriction value.</param>
public sealed record Restriction(int Id, string Name, string Value);
