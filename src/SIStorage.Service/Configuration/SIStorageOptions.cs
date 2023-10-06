namespace SIStorage.Service.Configuration;

/// <summary>
/// Provides options for SIStorage service.
/// </summary>
public sealed class SIStorageOptions
{
    public const string ConfigurationSectionName = "SIStorage";

    /// <summary>
    /// Folder for storing service content.
    /// </summary>
    public string ContentFolder { get; set; } = @".\wwwroot";

    /// <summary>
    /// Storage public Uri for package download.
    /// </summary>
    public Uri? PackageUri { get; set; }

    /// <summary>
    /// Storage public Uri for logo download.
    /// </summary>
    public Uri? LogoUri { get; set; }

    /// <summary>
    /// Service public uri.
    /// </summary>
    public Uri? PublicUri { get; set; }

    /// <summary>
    /// Should the service serve static content by itself.
    /// </summary>
    public bool ServeStaticFiles { get; set; } = true;
}
