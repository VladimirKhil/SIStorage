namespace SIStorage.Service.Contract.Models;

/// <summary>
/// Provides a package publisher info.
/// </summary>
/// <param name="Id">Publisher identifier.</param>
/// <param name="Name">Publisher name.</param>
public sealed record Publisher(int Id, string Name);
