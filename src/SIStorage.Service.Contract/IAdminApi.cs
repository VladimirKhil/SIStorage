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
}
