using NUnit.Framework;

namespace SIStorage.Service.IntegrationTests.Deep;

public sealed class PackagesApiTests : DeepTestsBase
{
    [Test]
    public async Task GetPackage_Ok()
    {
        var expectedPackage = DbConnection.Packages.First(p => p.Id == Package1Id);

        var package = await SIStorageClient.Packages.GetPackageAsync(Package1Id);

        Assert.NotNull(package);

        Assert.Multiple(() =>
        {
            Assert.AreEqual(expectedPackage.Id, package.Id);
            Assert.AreEqual(expectedPackage.Name, package.Name);
            Assert.AreEqual(expectedPackage.Size, package.Size);
            Assert.AreEqual(expectedPackage.DownloadCount, package.DownloadCount);
            Assert.AreEqual(expectedPackage.QuestionCount, package.QuestionCount);
            Assert.AreEqual(expectedPackage.AtomTypesStatistic, package.AtomTypesStatistic);
            Assert.IsNotNull(package.ContentUri);
            Assert.AreEqual(expectedPackage.CreateDate, package.CreateDate);
            Assert.AreEqual(expectedPackage.Difficulty, package.Difficulty);
            Assert.AreEqual(expectedPackage.LanguageId, package.LanguageId);
            Assert.AreEqual(expectedPackage.LogoUri, package.LogoUri);
            Assert.AreEqual(expectedPackage.PublisherId, package.PublisherId);
            Assert.AreEqual(expectedPackage.Rounds[0]!.Name, package.Rounds[0]!.Name);
            CollectionAssert.AreEqual(expectedPackage.Rounds[0].ThemeNames, package.Rounds[0].ThemeNames);
        });
    }

    [Test]
    public async Task GetPackage_Restriction_Free_Ok()
    {
        var expectedPackage = DbConnection.Packages.First();

        var package = await SIStorageClient.Packages.GetPackageAsync(expectedPackage.Id);

        Assert.NotNull(package);

        Assert.AreEqual(expectedPackage.Id, package.Id);
    }
}