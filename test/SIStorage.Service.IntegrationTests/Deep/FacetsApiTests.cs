using NUnit.Framework;
using SIStorage.Service.Contract.Models;

namespace SIStorage.Service.IntegrationTests.Deep;

public sealed class FacetsApiTests : DeepTestsBase
{
    [Test]
    public async Task GetTags_Ok()
    {
        var tags = await FacetsApi.GetTagsAsync();

        CollectionAssert.AreEquivalent(
            new[]
            {
                new Tag(1, "tag1"),
                new Tag(2, "tag2"),
                new Tag(3, "tag3"),
                new Tag(4, "tag4"),
            },
            tags);
    }

    [Test]
    public async Task GetTags_WithLanguage_Ok()
    {
        var tags = await FacetsApi.GetTagsAsync(CommonLanguageId);

        CollectionAssert.AreEquivalent(
            new[]
            {
                new Tag(1, "tag1"),
                new Tag(2, "tag2"),
                new Tag(3, "tag3"),
            },
            tags);
    }

    [Test]
    public async Task GetPublishers_Ok()
    {
        var publishers = await FacetsApi.GetPublishersAsync();

        CollectionAssert.AreEquivalent(
            new[]
            {
                new Publisher(1, "publisher1"),
                new Publisher(2, "publisher2"),
                new Publisher(3, "publisher3"),
            },
            publishers);
    }

    [Test]
    public async Task GetPublishers_WithLanguage_Ok()
    {
        var publishers = await FacetsApi.GetPublishersAsync(CommonLanguageId);

        CollectionAssert.AreEquivalent(
            new[]
            {
                new Publisher(1, "publisher1"),
                new Publisher(2, "publisher2"),
            },
            publishers);
    }

    [Test]
    public async Task GetAuthors_Ok()
    {
        var authors = await FacetsApi.GetAuthorsAsync();

        CollectionAssert.AreEquivalent(
            new[]
            {
                new Author(1, "author1"),
                new Author(2, "author2"),
                new Author(3, "author3"),
            },
            authors);
    }

    [Test]
    public async Task GetAuthors_WithLanguage_Ok()
    {
        var authors = await FacetsApi.GetAuthorsAsync(CommonLanguageId);

        CollectionAssert.AreEquivalent(
            new[]
            {
                new Author(1, "author1"),
                new Author(2, "author2"),
            },
            authors);
    }

    [Test]
    public async Task GetLanguages_Ok()
    {
        var languages = await FacetsApi.GetLanguagesAsync();

        CollectionAssert.AreEquivalent(
            new[]
            {
                new Language(1, "language1"),
                new Language(2, "language2"),
            },
            languages);
    }

    [Test]
    public async Task GetRestriction_Ok()
    {
        var restrictions = await FacetsApi.GetRestrictionsAsync();

        CollectionAssert.AreEquivalent(
            new[]
            {
                new Restriction(1, "Age", "12+"),
                new Restriction(2, "Age", "18+"),
            },
            restrictions);
    }
}