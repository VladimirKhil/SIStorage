using SIStorage.Service.Contract;
using SIStorage.Service.Contract.Responses;
using System.Net.Http.Json;

namespace SIStorage.Service.Client;

internal sealed class InfoApi : IInfoApi
{
    private readonly HttpClient _client;

    /// <summary>
    /// Initializes a new instance of <see cref="InfoApi" /> class.
    /// </summary>
    /// <param name="client">HTTP client to use.</param>
    public InfoApi(HttpClient client) => _client = client;

    public Task<StorageInfo?> GetInfoAsync(CancellationToken cancellationToken = default) =>
        _client.GetFromJsonAsync<StorageInfo>("info", cancellationToken: cancellationToken);

}
