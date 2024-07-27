using Microsoft.AspNetCore.Mvc;
using SIStorage.Service.Contract.Models;
using SIStorage.Service.Contract.Requests;
using SIStorage.Service.Contract.Responses;
using SIStorage.Service.Contracts;

namespace SIStorage.Service.Controllers;

/// <summary>
/// Provides API for working with packages.
/// </summary>
/// <remarks>
/// Initializes a new instance of <see cref="PackagesController" />.
/// </remarks>
/// <param name="packagesApi">Packages API.</param>
[Route("api/v1/packages")]
[ApiController]
[Produces("application/json")]
public sealed class PackagesController(IExtendedPackagesApi packagesApi) : ControllerBase
{
    /// <summary>
    /// Gets package by identifier.
    /// </summary>
    /// <param name="packageId">Package identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Package information.</returns>
    [HttpGet("{packageId}")]
    public Task<Package> GetAsync(Guid packageId, CancellationToken cancellationToken) =>
        packagesApi.GetPackageAsync(packageId, cancellationToken);

    /// <remarks>
    /// This API is not included in client library. It is called implicitly when trying to download the package.
    /// </remarks>
    /// <summary>
    /// Increments package download counter and redirects to direct package file download link.
    /// </summary>
    [HttpGet("{packageId}/download")]
    public async Task<IActionResult> GetDownloadLinkAsync(Guid packageId, string callbackUri, CancellationToken cancellationToken)
    {
        await packagesApi.IncrementDownloadCountAsync(packageId, cancellationToken);

        return Redirect(callbackUri);
    }

    /// <summary>
    /// Seaches packages by facets identifiers.
    /// </summary>
    /// <param name="sortMode">Sort mode: name, create date, download count, rating.</param>
    /// <param name="sortDirection">Sort direction: ascending, descending.</param>
    /// <param name="from">Pagination from value.</param>
    /// <param name="count">Pagination count value.</param>
    /// <param name="tagIds">Tags identifiers (-1 if packages should not contain any tags).</param>
    /// <param name="difficulty">Difficulty.</param>
    /// <param name="difficultyCompareMode">Difficulty compare mode flags (greater than <paramref name="difficulty" />, equal to, less than).</param>
    /// <param name="publisherId">Publisher identifier (-1 if packages should not have publisher).</param>
    /// <param name="authorId">Authors identifiers (-1 if packages should not contain any authors).</param>
    /// <param name="restrictionIds">Restrictions identifiers (-1 if packages should not contain any restrictions).</param>
    /// <param name="languageId">Lnaguage identifier.</param>
    /// <param name="searchText">Text to contain.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Found packages page.</returns>
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

        return packagesApi.GetPackagesAsync(packageFilters, packageSelectionParameters, cancellationToken);
    }

    /// <summary>
    /// Seaches packages by facets values.
    /// </summary>
    /// <param name="sortMode">Sort mode: name, create date, download count, rating.</param>
    /// <param name="sortDirection">Sort direction: ascending, descending.</param>
    /// <param name="from">Pagination from value.</param>
    /// <param name="count">Pagination count value.</param>
    /// <param name="tags">Tags comma separated string (_ if packages should not contain any tags).</param>
    /// <param name="difficulty">Difficulty.</param>
    /// <param name="difficultyCompareMode">Difficulty compare mode flags (greater than <paramref name="difficulty" />, equal to, less than).</param>
    /// <param name="publisher">Publisher (_ if packages should not have publisher).</param>
    /// <param name="author">Authors comma separated string (_ if packages should not contain any authors).</param>
    /// <param name="restrictions">Restrictions comma separated string of kind 'name:value' (_ if packages should not contain any restrictions).</param>
    /// <param name="language">Lnaguage.</param>
    /// <param name="searchText">Text to contain.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Found packages page.</returns>
    [HttpGet("search")]
    public Task<PackagesPage> SearchByValuesAsync(
        PackageSortMode sortMode,
        PackageSortDirection sortDirection,
        int from,
        int count,
        string? tags = null,
        short? difficulty = null,
        CompareMode? difficultyCompareMode = null,
        string? publisher = null,
        string? author = null,
        string? restrictions = null,
        string? language = null,
        string? searchText = null,
        CancellationToken cancellationToken = default) => throw new NotImplementedException();

    /// <summary>
    /// Creates temporary random package.
    /// </summary>
    /// <param name="packageParameters">Random package parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Created package info.</returns>
    [HttpPost("random")]
    public Task<Package> PostRandomAsync(
        RandomPackageParameters packageParameters,
        CancellationToken cancellationToken = default) => packagesApi.GetRandomPackageAsync(packageParameters, cancellationToken);
}
