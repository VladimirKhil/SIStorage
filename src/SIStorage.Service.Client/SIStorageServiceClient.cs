using SIStorage.Service.Contract;

namespace SIStorage.Service.Client;

/// <inheritdoc cref="ISIStorageServiceClient" />
internal sealed class SIStorageServiceClient : ISIStorageServiceClient
{
    public IInfoApi Info { get; }

    public IFacetsApi Facets { get; }

    public IPackagesApi Packages { get; }

    public IAdminApi Admin { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="SIStorageServiceClient" /> class.
    /// </summary>
    /// <param name="client">HTTP client to use.</param>
    public SIStorageServiceClient(HttpClient client)
    {
        Info = new InfoApi(client);
        Facets = new FacetsApi(client);
        Packages = new PackagesApi(client);
        Admin = new AdminApi(client);
    }
}
