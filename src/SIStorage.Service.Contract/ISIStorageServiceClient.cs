namespace SIStorage.Service.Contract;

/// <summary>
/// Defines a SIStorage client.
/// </summary>
public interface ISIStorageServiceClient
{
    /// <summary>
    /// API for working with facets.
    /// </summary>
    IFacetsApi Facets { get; }

    /// <summary>
    /// API for working with packages.
    /// </summary>
    IPackagesApi Packages { get; }
}
