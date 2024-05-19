using SIStorage.Service.Contract;
using System.Net.Http.Headers;
using System.Text;

namespace SIStorage.Service.Client;

/// <inheritdoc />
internal sealed class SIStorageClientFactory : ISIStorageClientFactory
{
    private readonly IHttpClientFactory _httpClientFactory;

    public SIStorageClientFactory(IHttpClientFactory httpClientFactory) => _httpClientFactory = httpClientFactory;

    public ISIStorageServiceClient CreateClient(Uri? serviceUri = null, string? clientSecret = null)
    {
        var httpClient = _httpClientFactory.CreateClient(nameof(ISIStorageServiceClient));

        if (serviceUri != null)
        {
            httpClient.BaseAddress = serviceUri;
        }

        if (clientSecret != null)
        {
            var authHeader = Convert.ToBase64String(Encoding.ASCII.GetBytes($"admin:{clientSecret}"));
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authHeader);
        }

        return new SIStorageServiceClient(httpClient);
    }
}
