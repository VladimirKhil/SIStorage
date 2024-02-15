using NUnit.Framework;
using SIStorage.Service.Contract.Requests;

namespace SIStorage.Service.IntegrationTests;

internal sealed class RandomPackagesTests : TestsBase
{
    [Test]
    public async Task GeneratePackage_Ok()
    {
        var package = await PackagesApi.GetRandomPackageAsync(new RandomPackageParameters());
        Assert.That(package, Is.Not.Null);
    }
}
