using SIStorage.Service.Contract.Models;
using SIStorage.Service.Contract.Requests;
using SIStorage.Service.Contract.Responses;

namespace SIStorage.Service.Contract;

/// <summary>
/// Provides API for admin operations.
/// </summary>
public interface IAdminApi
{
    /// <summary>
    /// Uploads package to service.
    /// </summary>
    /// <param name="packageName">Package name.</param>
    /// <param name="packageStream">Package stream.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<CreatePackageResponse> UploadPackageAsync(string packageName, Stream packageStream, CancellationToken cancellationToken = default);

    /// <summary>
    /// Reindexes storage.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task ReindexAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets random package.
    /// </summary>
    /// <param name="randomPackageParameters">Random package parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<Package> GetRandomPackageAsync(
        RandomPackageParameters randomPackageParameters,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes package from storage.
    /// </summary>
    /// <param name="packageId">Package identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task DeletePackageAsync(Guid packageId, CancellationToken cancellationToken = default);
}
