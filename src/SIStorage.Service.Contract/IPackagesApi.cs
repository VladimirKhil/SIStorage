using SIStorage.Service.Contract.Models;
using SIStorage.Service.Contract.Requests;
using SIStorage.Service.Contract.Responses;

namespace SIStorage.Service.Contract;

/// <summary>
/// Provides API for working with packages.
/// </summary>
public interface IPackagesApi
{
    /// <summary>
    /// Gets package by id.
    /// </summary>
    /// <param name="packageId">Package id.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<Package> GetPackageAsync(
        Guid packageId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets (searches) packages by filters.
    /// </summary>
    /// <param name="packageFilters">Package filters.</param>
    /// <param name="packageSelectionParameters">Package sort and pagination parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<PackagesPage> GetPackagesAsync(
        PackageFilters packageFilters,
        PackageSelectionParameters packageSelectionParameters,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets random package.
    /// </summary>
    /// <param name="randomPackageParameters">Random package parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<Package> GetRandomPackageAsync(
        RandomPackageParameters randomPackageParameters,
        CancellationToken cancellationToken = default);
}
