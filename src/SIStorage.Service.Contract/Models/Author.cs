namespace SIStorage.Service.Contract.Models;

/// <summary>
/// Provides a package author info.
/// </summary>
/// <param name="Id">Author identifier.</param>
/// <param name="Name">Author name.</param>
public sealed record Author(int Id, string Name);
