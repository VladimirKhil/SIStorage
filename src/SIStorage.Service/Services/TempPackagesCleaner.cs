using Microsoft.Extensions.Options;
using SIStorage.Service.Configuration;
using SIStorage.Service.Contracts;

namespace SIStorage.Service.Services;

/// <summary>
/// Periodically cleans temporary packages.
/// </summary>
internal sealed class TempPackagesCleaner : BackgroundService
{
    private readonly ITempPackagesService _tempPackagesService;
    private readonly SIStorageOptions _options;
    private readonly ILogger<TempPackagesCleaner> _logger;

    public TempPackagesCleaner(
        ITempPackagesService tempPackagesService,
        IOptions<SIStorageOptions> options,
        ILogger<TempPackagesCleaner> logger)
    {
        _tempPackagesService = tempPackagesService;
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
            _tempPackagesService.Clean();
            await Task.Delay(_options.CleaningInterval, stoppingToken);
        }
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Cleaning service stopped");
        return base.StopAsync(cancellationToken);
    }
}
