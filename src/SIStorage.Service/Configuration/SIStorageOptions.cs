namespace SIStorage.Service.Configuration;

/// <summary>
/// Provides options for SIStorage service.
/// </summary>
public sealed class SIStorageOptions
{
    /// <summary>
    /// Default configuration section name.
    /// </summary>
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
    /// Storage public Uri for temporary package download.
    /// </summary>
    public Uri? TempUri { get; set; }

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

    /// <summary>
    /// Packages cleaning interval.
    /// </summary>
    public TimeSpan CleaningInterval { get; set; } = TimeSpan.FromMinutes(30);

    /// <summary>
    /// Temporary package lifetime.
    /// </summary>
    public TimeSpan TempPackageLifetime { get; set; } = TimeSpan.FromMinutes(10);
}
