namespace SIStorage.Service.Client;

/// <summary>
/// Provides options for <see cref="SIStorageServiceClient" /> class.
/// </summary>
internal sealed class SIStorageClientOptions
{
    /// <summary>
    /// Name of the configuration section holding these options.
    /// </summary>
    public const string ConfigurationSectionName = "SIStorageServiceClient";

    public const int DefaultRetryCount = 3;

    /// <summary>
    /// SIStorage service Uri.
    /// </summary>
    public Uri? ServiceUri { get; set; }

    /// <summary>
    /// Retry count policy.
    /// </summary>
    public int RetryCount { get; set; } = DefaultRetryCount;
}
