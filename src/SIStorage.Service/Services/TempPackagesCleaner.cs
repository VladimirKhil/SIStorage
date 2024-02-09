using Microsoft.Extensions.Options;
using SIStorage.Service.Configuration;
using SIStorage.Service.Contracts;

namespace SIStorage.Service.Services;

/// <summary>
/// Periodically cleans temporary packages.
/// </summary>
internal sealed class TempPackagesCleaner : BackgroundService
{
    private readonly IExtendedPackagesApi _packagesApi;
    private readonly SIStorageOptions _options;
    private readonly ILogger<TempPackagesCleaner> _logger;

    public TempPackagesCleaner(
        IExtendedPackagesApi packagesApi,
        IOptions<SIStorageOptions> options,
        ILogger<TempPackagesCleaner> logger)
    {
        _packagesApi = packagesApi;
        _options = options.Value;
        _logger = logger;
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Cleaning service started");
        return base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _packagesApi.CleanTempPackages();
            await Task.Delay(_options.CleaningInterval, stoppingToken);
        }
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Cleaning service stopped");
        return base.StopAsync(cancellationToken);
    }
}
