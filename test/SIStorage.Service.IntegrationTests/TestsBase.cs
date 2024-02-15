using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SIStorage.Service.Client;
using SIStorage.Service.Contract;

namespace SIStorage.Service.IntegrationTests;

/// <summary>
/// Provides base methods for SIStorage service integration tests.
/// </summary>
/// <remarks>
public abstract class TestsBase
{
    protected ISIStorageServiceClient SIStorageClient { get; }

    protected IFacetsApi FacetsApi => SIStorageClient.Facets;

    protected IPackagesApi PackagesApi => SIStorageClient.Packages;

    protected IAdminApi AdminApi => SIStorageClient.Admin;

    public TestsBase()
    {
        var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
        var configuration = builder.Build();

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddSIStorageServiceClient(configuration);

        var serviceProvider = serviceCollection.BuildServiceProvider();
        SIStorageClient = serviceProvider.GetRequiredService<ISIStorageServiceClient>();
    }
}
