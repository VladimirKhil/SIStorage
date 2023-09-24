namespace SIStorage.Service.Contract.Models;

/// <summary>
/// Provides a package language info.
/// </summary>
/// <param name="Id"> Language identifier.</param>
/// <param name="Code">Language code.</param>
public sealed record Language(int Id, string Code);
