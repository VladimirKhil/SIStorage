using SIStorage.Service.Contract;

namespace SIStorage.Service.Client;

/// <summary>
/// Provides method for creating custom SIStorageService clients.
/// </summary>
public interface ISIStorageClientFactory
{
    /// <summary>
    /// Creates SIStorageService client with custom service uri.
    /// </summary>
    /// <param name="serviceUri">Service uri.</param>
    /// <param name="clientSecret">Optional client secret.</param>
    ISIStorageServiceClient CreateClient(Uri? serviceUri = null, string? clientSecret = null);
}
