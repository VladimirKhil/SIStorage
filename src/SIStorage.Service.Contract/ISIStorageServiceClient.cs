namespace SIStorage.Service.Contract;

/// <summary>
/// Defines a SIStorage client.
/// </summary>
public interface ISIStorageServiceClient
{
    /// <summary>
    /// API for getting storage info.
    /// </summary>
    IInfoApi Info { get; }

    /// <summary>
    /// API for working with facets.
    /// </summary>
    IFacetsApi Facets { get; }

    /// <summary>
    /// API for working with packages.
    /// </summary>
    IPackagesApi Packages { get; }

    /// <summary>
    /// API for admin operations.
    /// </summary>
    IAdminApi Admin { get; }
}
