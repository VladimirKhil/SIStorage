namespace SIStorage.Service.Contract.Models;

/// <summary>
/// Defines a SIStorageService error.
/// </summary>
public sealed class SIStorageServiceError
{
    /// <summary>
    /// Error code.
    /// </summary>
    public WellKnownSIStorageServiceErrorCode ErrorCode { get; set; }
}
