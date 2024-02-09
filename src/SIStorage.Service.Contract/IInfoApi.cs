using SIStorage.Service.Contract.Responses;

namespace SIStorage.Service.Contract;

/// <summary>
/// Allows to get storage info.
/// </summary>
public interface IInfoApi
{
    /// <summary>
    /// Gets storage info.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<StorageInfo?> GetInfoAsync(CancellationToken cancellationToken = default);
}
