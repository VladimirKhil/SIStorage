using SIStorage.Service.Contract;
using SIStorage.Service.Contract.Models;
using System.Net.Http.Json;

namespace SIStorage.Service.Client;

internal sealed class FacetsApi : IFacetsApi
{
    private readonly HttpClient _client;

    /// <summary>
    /// Initializes a new instance of <see cref="FacetsApi" /> class.
    /// </summary>
    /// <param name="client">HTTP client to use.</param>
    public FacetsApi(HttpClient client) => _client = client;

    public async Task<Publisher[]> GetPublishersAsync(int? languageId = null, CancellationToken cancellationToken = default) =>
        (await _client.GetFromJsonAsync<Publisher[]>($"facets/publishers{CreateLanguageParam(languageId)}", cancellationToken)) ?? Array.Empty<Publisher>();

    public async Task<Author[]> GetAuthorsAsync(int? languageId = null, CancellationToken cancellationToken = default) =>
        (await _client.GetFromJsonAsync<Author[]>($"facets/authors{CreateLanguageParam(languageId)}", cancellationToken)) ?? Array.Empty<Author>();

    public async Task<Tag[]> GetTagsAsync(int? languageId = null, CancellationToken cancellationToken = default) =>
        (await _client.GetFromJsonAsync<Tag[]>($"facets/tags{CreateLanguageParam(languageId)}", cancellationToken)) ?? Array.Empty<Tag>();

    public async Task<Restriction[]> GetRestrictionsAsync(CancellationToken cancellationToken = default) =>
        (await _client.GetFromJsonAsync<Restriction[]>("facets/restrictions", cancellationToken)) ?? Array.Empty<Restriction>();

    public async Task<Language[]> GetLanguagesAsync(CancellationToken cancellationToken = default) =>
        (await _client.GetFromJsonAsync<Language[]>("facets/languages", cancellationToken)) ?? Array.Empty<Language>();

    private static string CreateLanguageParam(int? languageId) => languageId.HasValue ? $"?languageId={languageId.Value}" : "";
}
