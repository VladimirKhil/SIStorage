using NUnit.Framework;
using SIStorage.Service.Contract.Responses;
using System.Net;

namespace SIStorage.Service.IntegrationTests.Shallow;

internal sealed class UploadPackageTests : ShallowTestsBase
{
    private static readonly HttpClient Client = new() { DefaultRequestVersion = HttpVersion.Version20 };

    [Test]
    public async Task UploadPackage_Ok()
    {
        CreatePackageResponse packageResponse;

        using (var stream = File.OpenRead("TestPackage.siq"))
        {
            packageResponse = await AdminApi.UploadPackageAsync("test-package", stream);
        }

        var packageId = packageResponse.PackageId;

        var package = await PackagesApi.GetPackageAsync(packageId);

        Assert.Multiple(() =>
        {
            Assert.That(package.Id, Is.EqualTo(packageId));
            Assert.That(package.RestrictionIds, Is.Not.Null);
            Assert.That(package.ContentTypeStatistic, Is.Not.Null);
            Assert.That(package.ContentTypeStatistic, Has.Count.EqualTo(2));
            Assert.That(package.ContentTypeStatistic, Contains.Key("image"));
            Assert.That(package.ContentTypeStatistic, Contains.Key("audio"));
            Assert.That(package.ContentTypeStatistic!["image"], Is.EqualTo(1));
            Assert.That(package.QuestionCount, Is.EqualTo(5));
            Assert.That(package.Difficulty, Is.EqualTo(6));
            Assert.That(package.DownloadCount, Is.EqualTo(0));
            Assert.That(package.Name, Is.EqualTo("Вопросы SIGame"));
            Assert.That(package.Rating, Is.EqualTo(0));
            Assert.That(package.Rounds, Is.Not.Null);
            Assert.That(package.Rounds, Has.Length.EqualTo(3));
            Assert.That(package.Rounds![0].Name, Is.EqualTo("Round 1"));
            Assert.That(package.Rounds![0].ThemeNames, Is.EquivalentTo(new[] { "Theme", "Theme2" }));
            Assert.That(package.Size, Is.EqualTo(37880));
        });

        var languages = await FacetsApi.GetLanguagesAsync();
        var packageLanguage = languages.FirstOrDefault(l => l.Id == package.LanguageId);

        Assert.That(packageLanguage, Is.Not.Null);
        Assert.That(packageLanguage!.Code, Is.EqualTo("ru-RU"));

        var authors = await FacetsApi.GetAuthorsAsync();
        var packageAuthors = authors.Where(a => package.AuthorIds.Contains(a.Id)).ToArray();

        Assert.That(packageAuthors, Has.Length.EqualTo(1));
        Assert.That(packageAuthors[0].Name, Is.EqualTo("Vladimir Khil"));

        var publihers = await FacetsApi.GetPublishersAsync();
        var packagePublisher = publihers.FirstOrDefault(p => p.Id == package.PublisherId);

        Assert.That(packagePublisher, Is.Not.Null);
        Assert.That(packagePublisher!.Name, Is.EqualTo("Special"));

        var restrictions = await FacetsApi.GetRestrictionsAsync();
        var packageRestrictions = restrictions.Where(r => package.RestrictionIds!.Contains(r.Id)).ToArray();

        Assert.That(packageRestrictions, Has.Length.EqualTo(1));
        Assert.That(packageRestrictions[0].Name, Is.EqualTo("Age"));
        Assert.That(packageRestrictions[0].Value, Is.EqualTo("18+"));

        var tags = await FacetsApi.GetTagsAsync();
        var packageTags = tags.Where(t => package.TagIds.Contains(t.Id)).ToArray();

        Assert.That(packageTags, Has.Length.EqualTo(2));
        Assert.That(packageTags.Select(t => t.Name), Is.EquivalentTo(new[] { "tag1", "tag2" }));

        using var response = await Client.GetAsync(package.ContentUri);
        Assert.That(response.IsSuccessStatusCode, Is.True);
        Assert.That(response.Content.Headers.ContentLength, Is.EqualTo(37880));

        using var directResponse = await Client.GetAsync(package.DirectContentUri);
        Assert.That(directResponse.IsSuccessStatusCode, Is.True);
        Assert.That(directResponse.Content.Headers.ContentLength, Is.EqualTo(37880));

        var package2 = await PackagesApi.GetPackageAsync(packageId);
        Assert.That(package2.DownloadCount, Is.EqualTo(1));

        using var logoResponse = await Client.GetAsync(package.LogoUri);
        Assert.That(logoResponse.IsSuccessStatusCode, Is.True);
        Assert.That(logoResponse.Content.Headers.ContentLength, Is.EqualTo(27187));
    }
}
