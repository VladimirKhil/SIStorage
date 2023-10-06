namespace SIStorage.Service.Contract.Models;

/// <summary>
/// Defines well-known SIStorage service error codes.
/// </summary>
public enum WellKnownSIStorageServiceErrorCode
{
    /// <summary>
    /// File is empty.
    /// </summary>
    FileEmpty,

    /// <summary>
    /// File is too large.
    /// </summary>
    FileTooLarge,

    /// <summary>
    /// Multipart ContentType is required.
    /// </summary>
    MultipartContentTypeRequired,

    /// <summary>
    /// Content-Length header is required.
    /// </summary>
    ContentLengthHeaderRequired,

    /// <summary>
    /// Content-Disposition header is required.
    /// </summary>
    ContentDispositionHeaderRequired,

    /// <summary>
    /// Disposition FileName is required.
    /// </summary>
    DispositionFileNameRequired,

    /// <summary>
    /// Package not found.
    /// </summary>
    PackageNotFound,
}
