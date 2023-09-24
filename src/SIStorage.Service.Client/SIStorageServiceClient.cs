using SIStorage.Service.Contract;

namespace SIStorage.Service.Client;

/// <inheritdoc cref="ISIStorageServiceClient" />
internal sealed class SIStorageServiceClient : ISIStorageServiceClient
{
    public IFacetsApi Facets { get; }

    public IPackagesApi Packages { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="SIStorageServiceClient" /> class.
    /// </summary>
    /// <param name="client">HTTP client to use.</param>
    public SIStorageServiceClient(HttpClient client)
    {
        Facets = new FacetsApi(client);
        Packages = new PackagesApi(client);
    }
}
