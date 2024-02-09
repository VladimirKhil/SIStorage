﻿using SIStorage.Service.Contract;
using SIStorage.Service.Models;

namespace SIStorage.Service.Contracts;

/// <summary>
/// Provides additional API for working with packages.
/// </summary>
public interface IExtendedPackagesApi : IPackagesApi
{
    /// <summary>
    /// Adds new package to storage.
    /// </summary>
    /// <param name="packageId">Package identifier.</param>
    /// <param name="packageName">Package name.</param>
    /// <param name="packageMetadata">Package info.</param>
    /// <param name="packageFileName">Package file name.</param>
    /// <param name="packageFileSize">Package file size.</param>
    /// <param name="logoUri">Package logo uri.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task AddPackageAsync(
        Guid packageId,
        string packageName,
        PackageInfo packageMetadata,
        string packageFileName,
        long packageFileSize,
        string? logoUri,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Increments package download count metric.
    /// </summary>
    /// <param name="packageId">Package identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task IncrementDownloadCountAsync(Guid packageId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Cleans temporary packages. 
    /// </summary>
    void CleanTempPackages();
}
