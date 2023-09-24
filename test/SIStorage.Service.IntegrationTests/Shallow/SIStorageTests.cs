using NUnit.Framework;
using SIStorage.Service.Contract.Models;
using SIStorage.Service.Contract.Requests;

namespace SIStorage.Service.IntegrationTests.Shallow;

internal sealed class SIStorageTests : ShallowTestsBase
{
    [Test]
    public async Task SearchByPublishers_Ok()
    {
        var publishers = await FacetsApi.GetPublishersAsync();

        if (!publishers.Any())
        {
            Assert.Ignore("No publishers found");
            return;
        }

        var publisherId = publishers.First().Id;

        await TestSearchAsync(
            new PackageFilters { PublisherId = publisherId },
            p => p.PublisherId == publisherId,
            new PackageSelectionParameters
            {
                Count = 10,
                SortDirection = PackageSortDirection.Ascending,
                SortMode = PackageSortMode.CreatedDate,
            });
    }

    [Test]
    public async Task SearchByTags_Ok()
    {
        var tags = await FacetsApi.GetTagsAsync();

        if (!tags.Any())
        {
            Assert.Ignore("No tags found");
            return;
        }

        var tagId = tags.First().Id;

        await TestSearchAsync(
            new PackageFilters { TagIds = new[] { tagId } },
            p => p.TagIds.Contains(tagId),
            new PackageSelectionParameters
            {
                Count = 5,
                SortDirection = PackageSortDirection.Descending,
                SortMode = PackageSortMode.Name,
            });
    }

    [Test]
    public async Task SearchByAuthors_Ok()
    {
        var authors = await FacetsApi.GetAuthorsAsync();

        if (!authors.Any())
        {
            Assert.Ignore("No authors found");
            return;
        }

        var authorId = authors.First().Id;

        await TestSearchAsync(
            new PackageFilters { AuthorId = authorId },
            p => p.AuthorIds.Contains(authorId),
            new PackageSelectionParameters
            {
                Count = 5,
                SortDirection = PackageSortDirection.Descending,
                SortMode = PackageSortMode.Name,
            });
    }

    [Test]
    public async Task SearchByRestrictions_Ok()
    {
        var restrictions = await FacetsApi.GetRestrictionsAsync();

        if (!restrictions.Any())
        {
            Assert.Ignore("No restrictions found");
            return;
        }

        var restrictionId = restrictions.First().Id;

        await TestSearchAsync(
            new PackageFilters { RestrictionIds = new[] { restrictionId } },
            p => p.RestrictionIds != null && p.RestrictionIds.Contains(restrictionId),
            new PackageSelectionParameters
            {
                Count = 5,
                SortDirection = PackageSortDirection.Ascending,
                SortMode = PackageSortMode.Name,
            });
    }

    [Test]
    public async Task SearchByDifficulty_LessThan_Ok()
    {
        var packages = await PackagesApi.GetPackagesAsync(
            new PackageFilters(),
            new PackageSelectionParameters());

        const int difficulty = 6;

        if (!packages.Packages.Any(p => p.Difficulty < difficulty))
        {
            Assert.Ignore("No packages found");
            return;
        }

        await TestSearchAsync(
            new PackageFilters
            {
                Difficulty = new NumericFilter<short>
                {
                    CompareMode = CompareMode.LessThan,
                    Value = difficulty
                }
            },
            p => p.Difficulty < difficulty,
            new PackageSelectionParameters
            {
                Count = 5,
                SortDirection = PackageSortDirection.Ascending,
                SortMode = PackageSortMode.Name,
            });
    }

    [Test]
    public async Task SearchByDifficulty_GreatenThanOrEqualTo_Ok()
    {
        var packages = await PackagesApi.GetPackagesAsync(
            new PackageFilters(),
            new PackageSelectionParameters());

        const int difficulty = 4;

        if (!packages.Packages.Any(p => p.Difficulty >= difficulty))
        {
            Assert.Ignore("No packages found");
            return;
        }

        await TestSearchAsync(
            new PackageFilters
            {
                Difficulty = new NumericFilter<short>
                {
                    CompareMode = CompareMode.GreaterThan | CompareMode.EqualTo,
                    Value = difficulty
                }
            },
            p => p.Difficulty >= difficulty,
            new PackageSelectionParameters
            {
                Count = 5,
                SortDirection = PackageSortDirection.Ascending,
                SortMode = PackageSortMode.Name,
            });
    }

    [Test]
    public async Task SearchByText_Ok()
    {
        var packages = await PackagesApi.GetPackagesAsync(
            new PackageFilters(),
            new PackageSelectionParameters());

        var package = packages.Packages.FirstOrDefault(p => p.Name != null && p.Name.Length > 3);

        if (package == null)
        {
            Assert.Ignore("No packages found");
            return;
        }

        var text = package.Name![..3];

        await TestSearchAsync(
            new PackageFilters { SearchText = text },
            p => p.Name != null && p.Name.Contains(text),
            new PackageSelectionParameters
            {
                Count = 5,
                SortDirection = PackageSortDirection.Ascending,
                SortMode = PackageSortMode.Name,
            });
    }

    [Test]
    public async Task SearchByLanguage_Ok()
    {
        var languages = await FacetsApi.GetLanguagesAsync();

        if (!languages.Any())
        {
            Assert.Ignore("No languages found");
            return;
        }

        var languageId = languages.First().Id;

        await TestSearchAsync(
            new PackageFilters { LanguageId = languageId },
            p => p.LanguageId == languageId,
            new PackageSelectionParameters
            {
                Count = 5,
                SortDirection = PackageSortDirection.Descending,
                SortMode = PackageSortMode.CreatedDate,
            });
    }

    private async Task TestSearchAsync(
        PackageFilters filter,
        Func<Package, bool> packageValidator,
        PackageSelectionParameters selectionParameters)
    {
        var page = await PackagesApi.GetPackagesAsync(filter, selectionParameters);

        if (page.Packages.Any())
        {
            Assert.Multiple(() =>
            {
                Assert.IsTrue(page.Packages.All(packageValidator), "Validator is wrong");
                Assert.Less(0, page.Total);

                Assert.GreaterOrEqual(selectionParameters.Count, page.Packages.Length);

                if (page.Packages.Length > 1)
                {
                    ValidateSorting(page.Packages, selectionParameters);
                }
            });
        }
        else
        {
            Assert.AreEqual(0, page.Total);
        }
    }

    private static void ValidateSorting(Package[] packages, PackageSelectionParameters selectionParameters)
    {
        if (selectionParameters.SortMode == PackageSortMode.CreatedDate)
        {
            var currentDate = selectionParameters.SortDirection == PackageSortDirection.Ascending
                ? DateOnly.MinValue
                : DateOnly.MaxValue;

            for (var i = 0; i < packages.Length; i++)
            {
                var packageDate = packages[i].CreateDate;

                if (!packageDate.HasValue)
                {
                    continue;
                }

                if (selectionParameters.SortDirection == PackageSortDirection.Ascending)
                {
                    Assert.LessOrEqual(currentDate, packageDate.Value);
                }
                else
                {
                    Assert.GreaterOrEqual(currentDate, packageDate.Value);
                }

                currentDate = packageDate.Value;
            }

            return;
        }

        if (selectionParameters.SortMode == PackageSortMode.Name)
        {
            var currentName = packages[0].Name ?? "";

            for (var i = 1; i < packages.Length; i++)
            {
                var packageName = packages[i].Name;

                if (packageName == null)
                {
                    continue;
                }

                if (selectionParameters.SortDirection == PackageSortDirection.Ascending)
                {
                    Assert.True(currentName.CompareTo(packageName) <= 0, "Wrong ascending sorting");
                }
                else
                {
                    Assert.True(currentName.CompareTo(packageName) >= 0, "Wrong descending sorting");
                }

                currentName = packageName;
            }

            return;
        }
    }
}
