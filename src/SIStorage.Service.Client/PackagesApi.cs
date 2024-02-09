using SIStorage.Service.Contract;
using SIStorage.Service.Contract.Models;
using SIStorage.Service.Contract.Requests;
using SIStorage.Service.Contract.Responses;
using System.Net;
using System.Net.Http.Json;

namespace SIStorage.Service.Client;

/// <inheritdoc />
internal sealed class PackagesApi : IPackagesApi
{
    private readonly HttpClient _client;

    /// <summary>
    /// Initializes a new instance of <see cref="SIStorageServiceClient" /> class.
    /// </summary>
    /// <param name="client">HTTP client to use.</param>
    public PackagesApi(HttpClient client) => _client = client;

    public async Task<Package> GetPackageAsync(
        Guid packageId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _client.GetFromJsonAsync<Package>($"packages/{packageId}", cancellationToken: cancellationToken)
                ?? throw new Exception(WellKnownSIStorageServiceErrorCode.PackageNotFound.ToString());
        }
        catch (HttpRequestException exc)
        {
            if (exc.StatusCode == HttpStatusCode.NotFound)
            {
                throw new Exception(WellKnownSIStorageServiceErrorCode.PackageNotFound.ToString());
            }

            throw;
        }
    }

    public async Task<PackagesPage> GetPackagesAsync(
        PackageFilters packageFilters,
        PackageSelectionParameters packageSelectionParameters,
        CancellationToken cancellationToken = default)
    {
        var query = new Dictionary<string, object>();

        if (packageFilters.TagIds != null)
        {
            query["tagIds"] = string.Join(",", packageFilters.TagIds);
        }

        if (packageFilters.Difficulty != null)
        {
            query["difficulty"] = packageFilters.Difficulty.Value;
            query["difficultyCompareMode"] = packageFilters.Difficulty.CompareMode;
        }

        if (packageFilters.PublisherId.HasValue)
        {
            query["publisherId"] = packageFilters.PublisherId.Value;
        }

        if (packageFilters.AuthorId.HasValue)
        {
            query["authorId"] = packageFilters.AuthorId.Value;
        }

        if (packageFilters.RestrictionIds != null && packageFilters.RestrictionIds.Length > 0)
        {
            query["restrictionIds"] = string.Join(",", packageFilters.RestrictionIds);
        }

        if (packageFilters.LanguageId.HasValue)
        {
            query["languageId"] = packageFilters.LanguageId.Value;
        }

        if (packageFilters.SearchText != null)
        {
            query["searchText"] = Uri.EscapeDataString(packageFilters.SearchText);
        }

        query["sortMode"] = packageSelectionParameters.SortMode;
        query["sortDirection"] = packageSelectionParameters.SortDirection;
        query["from"] = packageSelectionParameters.From;
        query["count"] = packageSelectionParameters.Count;

        var queryArgs = string.Join('&', query.Select(q => $"{q.Key}={q.Value}"));

        return (await _client.GetFromJsonAsync<PackagesPage>($"packages?{queryArgs}", cancellationToken)) ?? PackagesPage.Empty;
    }

    public async Task<Package> GetRandomPackageAsync(RandomPackageParameters randomPackageParameters, CancellationToken cancellationToken = default)
    {
        using var response = await _client.PostAsJsonAsync("packages/random", randomPackageParameters, cancellationToken: cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException(await response.Content.ReadAsStringAsync(cancellationToken), null, response.StatusCode);
        }

        var package = await response.Content.ReadFromJsonAsync<Package>(cancellationToken: cancellationToken)
            ?? throw new Exception(WellKnownSIStorageServiceErrorCode.PackageNotFound.ToString());

        return package;
    }
}
