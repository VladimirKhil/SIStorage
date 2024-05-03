namespace SIStorage.Service.Contract.Responses;

/// <summary>
/// Defines storage information.
/// </summary>
/// <param name="RandomPackagesSupported">Are random packages supported.</param>
/// <param name="IdentifiersSupported">Are integer identifiers supported.</param>
public sealed record StorageInfo(bool RandomPackagesSupported, bool IdentifiersSupported);
