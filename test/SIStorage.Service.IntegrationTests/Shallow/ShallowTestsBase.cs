using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SIStorage.Service.Client;
using SIStorage.Service.Contract;

namespace SIStorage.Service.IntegrationTests.Shallow;

/// <summary>
/// Provides base methods for SIStorage service integration tests.
/// </summary>
/// <remarks>
/// Storage service and PostgreSQL must be started before tests to run.
/// This tests class does not communicate with the database directly.
/// It assumes the the databse already has some data and makes several checks agains it.
/// It checks the correctness of SIStorage API.
/// </remarks>
public abstract class ShallowTestsBase
{
    protected ISIStorageServiceClient SIStorageClient { get; }

    protected IFacetsApi FacetsApi => SIStorageClient.Facets;

    protected IPackagesApi PackagesApi => SIStorageClient.Packages;

    public ShallowTestsBase()
    {
        var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
        var configuration = builder.Build();

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddSIStorageServiceClient(configuration);

        var serviceProvider = serviceCollection.BuildServiceProvider();
        SIStorageClient = serviceProvider.GetRequiredService<ISIStorageServiceClient>();
    }
}
