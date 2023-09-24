using SIStorage.Service.Contract;

namespace SIStorage.Service.Contracts;

/// <summary>
/// Provides additional API for working with packages.
/// </summary>
public interface IExtendedPackagesApi : IPackagesApi
{
    /// <summary>
    /// Increments package download count metric.
    /// </summary>
    /// <param name="packageId">Package identifier.</param>
    void IncrementDownloadCount(Guid packageId, CancellationToken cancellationToken = default);
}
