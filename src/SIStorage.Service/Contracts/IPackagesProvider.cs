using SIPackages;

namespace SIStorage.Service.Contracts;

/// <summary>
/// Provides package by identifier.
/// </summary>
internal interface IPackagesProvider
{
    /// <summary>
    /// Gets package by identifier.
    /// </summary>
    /// <param name="packageId">Package identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<SIDocument> GetPackageAsync(string packageId, CancellationToken cancellationToken = default);
}
