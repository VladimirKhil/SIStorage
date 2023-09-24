namespace SIStorage.Service.Configuration;

/// <summary>
/// Provides options for SIStorage service.
/// </summary>
public sealed class SIStorageOptions
{
    public const string ConfigurationSectionName = "SIStorage";

    /// <summary>
    /// Storage public Uri for package download.
    /// </summary>
    public Uri? PackageUri { get; set; }

    /// <summary>
    /// Storage public Uri for logo download.
    /// </summary>
    public Uri? LogoUri { get; set; }

    /// <summary>
    /// Callback Uri for counting the number of downloads.
    /// </summary>
    public Uri? DownloadCallbackUri { get; set; }
}
