using SIStorage.Service.Contract.Models;

namespace SIStorage.Service.ComponentTests;

public sealed class FacetsApiTests : TestsBase
{
    [Test]
    public async Task GetTags_Ok()
    {
        var tags = await FacetsApi.GetTagsAsync();

        Assert.That(
            tags,
            Is.EquivalentTo(
            new[]
            {
                new Tag(1, "tag1"),
                new Tag(2, "tag2"),
                new Tag(3, "tag3"),
                new Tag(4, "tag4"),
            }));
    }

    [Test]
    public async Task GetTags_WithLanguage_Ok()
    {
        var tags = await FacetsApi.GetTagsAsync(CommonLanguageId);

        Assert.That(
            tags,
            Is.EquivalentTo(
            new[]
            {
                new Tag(1, "tag1"),
                new Tag(2, "tag2"),
                new Tag(3, "tag3"),
            }));
    }

    [Test]
    public async Task GetPublishers_Ok()
    {
        var publishers = await FacetsApi.GetPublishersAsync();

        Assert.That(
            publishers,
            Is.EquivalentTo(
            new[]
            {
                new Publisher(1, "publisher1"),
                new Publisher(2, "publisher2"),
                new Publisher(3, "publisher3"),
            }));
    }

    [Test]
    public async Task GetPublishers_WithLanguage_Ok()
    {
        var publishers = await FacetsApi.GetPublishersAsync(CommonLanguageId);

        Assert.That(
            publishers,
            Is.EquivalentTo(
            new[]
            {
                new Publisher(1, "publisher1"),
                new Publisher(2, "publisher2"),
            }));
    }

    [Test]
    public async Task GetAuthors_Ok()
    {
        var authors = await FacetsApi.GetAuthorsAsync();

        Assert.That(
            authors,
            Is.EquivalentTo(
            new[]
            {
                new Author(1, "author1"),
                new Author(2, "author2"),
                new Author(3, "author3"),
            }));
    }

    [Test]
    public async Task GetAuthors_WithLanguage_Ok()
    {
        var authors = await FacetsApi.GetAuthorsAsync(CommonLanguageId);

        Assert.That(
            authors,
            Is.EquivalentTo(
            new[]
            {
                new Author(1, "author1"),
                new Author(2, "author2"),
            }));
    }

    [Test]
    public async Task GetLanguages_Ok()
    {
        var languages = await FacetsApi.GetLanguagesAsync();

        Assert.That(
            languages,
            Is.EquivalentTo(
            new[]
            {
                new Language(1, "language1"),
                new Language(2, "language2"),
            }));
    }

    [Test]
    public async Task GetRestriction_Ok()
    {
        var restrictions = await FacetsApi.GetRestrictionsAsync();
        
        Assert.That(
            restrictions,
            Is.EquivalentTo(
            new[]
            {
                new Restriction(1, "Age", "12+"),
                new Restriction(2, "Age", "18+"),
            }));
    }
}