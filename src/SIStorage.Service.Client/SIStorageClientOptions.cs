namespace SIStorage.Service.Client;

/// <summary>
/// Provides options for <see cref="SIStorageServiceClient" /> class.
/// </summary>
public sealed class SIStorageClientOptions
{
    /// <summary>
    /// Name of the configuration section holding these options.
    /// </summary>
    public const string ConfigurationSectionName = "SIStorageServiceClient";

    /// <summary>
    /// Default retry count value.
    /// </summary>
    public const int DefaultRetryCount = 3;

    /// <summary>
    /// SIStorage service Uri.
    /// </summary>
    public Uri? ServiceUri { get; set; }

    /// <summary>
    /// Secret to access restricted API.
    /// </summary>
    public string? ClientSecret { get; set; }

    /// <summary>
    /// Retry count policy.
    /// </summary>
    public int RetryCount { get; set; } = DefaultRetryCount;

    /// <summary>
    /// Client timeout.
    /// </summary>
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(300);
}
