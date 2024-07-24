namespace SIStorage.Service.ComponentTests;

public sealed class PackagesApiTests : TestsBase
{
    [Test]
    public async Task GetPackage_Ok()
    {
        var expectedPackage = DbConnection.Packages.First(p => p.Id == Package1Id);

        var package = await PackagesApi.GetPackageAsync(Package1Id);

        Assert.That(package, Is.Not.Null);

        Assert.Multiple(() =>
        {
            Assert.That(package.Id, Is.EqualTo(expectedPackage.Id));
            Assert.That(package.Name, Is.EqualTo(expectedPackage.Name));
            Assert.That(package.Size, Is.EqualTo(expectedPackage.Size));
            Assert.That(package.DownloadCount, Is.EqualTo(expectedPackage.DownloadCount));
            Assert.That(package.QuestionCount, Is.EqualTo(expectedPackage.QuestionCount));
            Assert.That(package.ContentTypeStatistic, Is.EqualTo(expectedPackage.ContentTypeStatistic));
            Assert.That(package.ContentUri, Is.Not.Null);
            Assert.That(package.CreateDate, Is.EqualTo(expectedPackage.CreateDate));
            Assert.That(package.Difficulty, Is.EqualTo(expectedPackage.Difficulty));
            Assert.That(package.LanguageId, Is.EqualTo(expectedPackage.LanguageId));
            Assert.That(package.LogoUri?.ToString(), Is.EqualTo(expectedPackage.LogoUri));
            Assert.That(package.PublisherId, Is.EqualTo(expectedPackage.PublisherId));
            Assert.That(package.Rounds[0].Name, Is.EqualTo(expectedPackage.Rounds[0]!.Name));
            Assert.That(expectedPackage.Rounds[0].ThemeNames, Is.EqualTo(package.Rounds[0].ThemeNames));
        });
    }

    [Test]
    public async Task GetPackage_Restriction_Free_Ok()
    {
        var expectedPackage = DbConnection.Packages.First();

        var package = await PackagesApi.GetPackageAsync(expectedPackage.Id);

        Assert.That(package, Is.Not.Null);
        Assert.That(package.Id, Is.EqualTo(expectedPackage.Id));
    }

    [Test]
    [Ignore("Run manually")]
    public async Task GetRandomPackage_Ok()
    {
        var package = await PackagesApi.GetRandomPackageAsync(new Contract.Requests.RandomPackageParameters
        {
            RestrictionIds = new int[] { -1 }
        });

        Assert.That(package, Is.Not.Null);
    }
}