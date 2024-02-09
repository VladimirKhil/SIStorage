using Microsoft.AspNetCore.Mvc;
using SIStorage.Service.Contract.Models;
using SIStorage.Service.Contract.Requests;
using SIStorage.Service.Contract.Responses;
using SIStorage.Service.Contracts;

namespace SIStorage.Service.Controllers;

[Route("api/v1/packages")]
[ApiController]
public sealed class PackagesController : ControllerBase
{
    private readonly IExtendedPackagesApi _packagesApi;

    public PackagesController(IExtendedPackagesApi packagesApi) => _packagesApi = packagesApi;

    [HttpGet("{packageId}")]
    public Task<Package> GetAsync(Guid packageId, CancellationToken cancellationToken) =>
        _packagesApi.GetPackageAsync(packageId, cancellationToken);

    /// <remarks>
    /// This API is not included in client library. It is called implicitly when trying to download the package.
    /// </remarks>
    [HttpGet("{packageId}/download")]
    public async Task<IActionResult> GetDownloadLinkAsync(Guid packageId, string callbackUri, CancellationToken cancellationToken)
    {
        await _packagesApi.IncrementDownloadCountAsync(packageId, cancellationToken);

        return Redirect(callbackUri);
    }

    [HttpGet]
    public Task<PackagesPage> GetAsync(
        PackageSortMode sortMode,
        PackageSortDirection sortDirection,
        int from,
        int count,
        string? tagIds = null,
        short? difficulty = null,
        CompareMode? difficultyCompareMode = null,
        int? publisherId = null,
        int? authorId = null,
        string? restrictionIds = null,
        int? languageId = null,
        string? searchText = null,
        CancellationToken cancellationToken = default)
    {
        var packageFilters = new PackageFilters
        {
            Difficulty = difficulty.HasValue
                ? new NumericFilter<short>
                {
                    Value = difficulty.Value,
                    CompareMode = difficultyCompareMode ?? CompareMode.GreaterThan | CompareMode.EqualTo
                }
                : null,

            PublisherId = publisherId,
            RestrictionIds = restrictionIds?.Split(',').Select(int.Parse).ToArray(),
            TagIds = tagIds?.Split(',').Select(int.Parse).ToArray(),
            AuthorId = authorId,
            LanguageId = languageId,
            SearchText = searchText
        };

        var packageSelectionParameters = new PackageSelectionParameters
        {
            SortMode = sortMode,
            SortDirection = sortDirection,
            From = from,
            Count = count
        };

        return _packagesApi.GetPackagesAsync(packageFilters, packageSelectionParameters, cancellationToken);
    }

    [HttpPost("random")]
    public Task<Package> PostRandomAsync(
        RandomPackageParameters packageParameters,
        CancellationToken cancellationToken = default)
    {
        return _packagesApi.GetRandomPackageAsync(packageParameters, cancellationToken);
    }
}
