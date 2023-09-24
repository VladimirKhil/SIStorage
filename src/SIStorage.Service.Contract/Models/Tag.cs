namespace SIStorage.Service.Contract.Models;

/// <summary>
/// Provides package tag info.
/// </summary>
/// <param name="Id">Tag identifier.</param>
/// <param name="Name">Tag name.</param>
public sealed record Tag(int Id, string Name);
