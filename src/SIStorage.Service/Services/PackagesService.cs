using LinqToDB;
using LinqToDB.Common;
using Microsoft.Extensions.Options;
using SIStorage.Database;
using SIStorage.Database.Models;
using SIStorage.Service.Configuration;
using SIStorage.Service.Contract.Common;
using SIStorage.Service.Contract.Models;
using SIStorage.Service.Contract.Requests;
using SIStorage.Service.Contract.Responses;
using SIStorage.Service.Contracts;
using SIStorage.Service.Exceptions;
using SIStorage.Service.Helpers;
using SIStorage.Service.Models;
using System.Linq.Expressions;
using System.Net;
using static LinqToDB.Sql;

namespace SIStorage.Service.Services;

/// <inheritdoc cref="IExtendedPackagesApi" />
internal sealed class PackagesService(
    ITempPackagesService tempPackagesService,
    SIStorageDbConnection connection,
    IOptions<SIStorageOptions> options,
    ILogger<PackagesService> logger) : IExtendedPackagesApi, IPackagesProvider
{
    private const string PackagesFolder = "packages";
    private const int RandomPackageSourceCount = 10;
    private readonly SIStorageOptions _options = options.Value;

    public async Task<Package> GetPackageAsync(Guid packageId, CancellationToken cancellationToken = default)
    {
        var query = EnrichPackageQuery(connection.Packages.Where(p => p.Id == packageId));
        var package = await query.FirstOrDefaultAsync(cancellationToken) ?? throw new ServiceException(WellKnownSIStorageServiceErrorCode.PackageNotFound, HttpStatusCode.NotFound);

        return ToPackage(package);
    }

    public async Task IncrementDownloadCountAsync(Guid packageId, CancellationToken cancellationToken = default)
    {
        try
        {
            await connection.Packages
                .Where(p => p.Id == packageId)
                .Set(p => p.DownloadCount, p => p.DownloadCount + 1)
                .UpdateAsync(cancellationToken);
        }
        catch (Exception exc)
        {
            logger.LogWarning(exc, "Error while incrementing download counter for package {packageId}: {message}", packageId, exc.Message);
        }
    }

    public async Task<PackagesPage> GetPackagesAsync(
        PackageFilters packageFilters,
        PackageSelectionParameters packageSelectionParameters,
        CancellationToken cancellationToken = default)
    {
        IQueryable<PackageModel> packages = connection.Packages;
        packages = BuildTagFilter(packageFilters.TagIds, packages);

        if (packageFilters.Difficulty != null)
        {
            packages = packages.Where(BuildDifficultyPredicate(packageFilters.Difficulty));
        }

        packages = BuildRestrictionFilter(packageFilters.RestrictionIds, packages);

        if (packageFilters.AuthorId != null)
        {
            packages = packages.Where(
                p => connection.PackageAuthors.Any(
                    pa => pa.PackageId == p.Id && packageFilters.AuthorId == pa.AuthorId));
        }

        if (packageFilters.PublisherId.HasValue)
        {
            if (packageFilters.PublisherId.Value == -1)
            {
                packages = packages.Where(p => p.PublisherId == null);
            }
            else
            {
                packages = packages.Where(p => p.PublisherId == packageFilters.PublisherId);
            }
        }

        packages = BuildLanguageFilter(packageFilters.LanguageId, packages);

        if (packageFilters.SearchText != null)
        {
            packages = packages.Where(p => p.Name!.Contains(packageFilters.SearchText));
        }

        if (packageSelectionParameters.SortDirection == PackageSortDirection.Ascending)
        {
            switch (packageSelectionParameters.SortMode)
            {
                case PackageSortMode.Name:
                    packages = packages.OrderBy(p => p.Name);
                    break;

                case PackageSortMode.CreatedDate:
                    packages = packages.OrderBy(p => p.CreateDate);
                    break;

                case PackageSortMode.DownloadCount:
                    packages = packages.OrderBy(p => p.DownloadCount);
                    break;

                default:
                    break;
            }
        }
        else
        {
            switch (packageSelectionParameters.SortMode)
            {
                case PackageSortMode.Name:
                    packages = packages.OrderByDescending(p => p.Name);
                    break;

                case PackageSortMode.CreatedDate:
                    packages = packages.OrderByDescending(p => p.CreateDate);
                    break;

                case PackageSortMode.DownloadCount:
                    packages = packages.OrderByDescending(p => p.DownloadCount);
                    break;

                default:
                    break;
            }
        }

        var query = EnrichPackageQuery(packages);

        var foundPackages = await query
            .Skip(packageSelectionParameters.From)
            .Take(packageSelectionParameters.Count)
            .ToArrayAsync(cancellationToken);

        var resultPackages = foundPackages.Select(ToPackage).ToArray();

        var totalPackages = await packages.CountAsync(cancellationToken);

        return new PackagesPage
        {
            Packages = resultPackages,
            Total = totalPackages,
        };
    }

    private static IQueryable<PackageModel> BuildLanguageFilter(int? languageId, IQueryable<PackageModel> packages) =>
        languageId.HasValue ? packages.Where(p => p.LanguageId == languageId) : packages;

    private IQueryable<PackageModel> BuildRestrictionFilter(int[]? restrictionIds, IQueryable<PackageModel> packages)
    {
        if (restrictionIds.IsNullOrEmpty())
        {
            return packages;
        }

        if (restrictionIds.Length == 1 && restrictionIds[0] == -1)
        {
            return packages.Where(p => !connection.PackageRestrictions.Any(pr => pr.PackageId == p.Id));
        }

        return packages.Where(
            p => connection.PackageRestrictions.Any(
                pr => pr.PackageId == p.Id && restrictionIds.Contains(pr.RestrictionId)));
    }

    private IQueryable<PackageModel> BuildTagFilter(int[]? tagIds, IQueryable<PackageModel> packages)
    {
        if (tagIds.IsNullOrEmpty())
        {
            return packages;
        }

        if (tagIds.Length == 1 && tagIds[0] == -1)
        {
            return packages.Where(p => !connection.PackageTags.Any(pt => pt.PackageId == p.Id));
        }

        return packages.Where(
            p => connection.PackageTags.Any(
                pt => pt.PackageId == p.Id && tagIds.Contains(pt.TagId)));
    }

    private Package ToPackage(EnrichedPackage enriched)
    {
        var package = enriched.Package;

        return new Package
        {
            ContentTypeStatistic = package.ContentTypeStatistic,
            AuthorIds = enriched.AuthorIds,
            ContentUri = package.Downloadable ? new Uri(BuildContentUri(package), UriKind.Absolute) : null,
            DirectContentUri = package.Downloadable ? new Uri(_options.PackageUri + package.FileName, UriKind.Absolute) : null,
            CreateDate = package.CreateDate,
            Difficulty = package.Difficulty,
            DownloadCount = package.DownloadCount,
            ExtraInfo = null,
            Id = package.Id,
            Rating = 0, // TODO
            LanguageId = package.LanguageId,
            LogoUri = package.LogoUri == null
                ? null
                : (Uri.TryCreate(package.LogoUri, UriKind.Absolute, out var uri) ? uri : new Uri(_options.LogoUri + package.LogoUri, UriKind.Absolute)),
            Name = package.Name,
            PublisherId = package.PublisherId,
            QuestionCount = package.QuestionCount,
            RestrictionIds = enriched.RestrictionIds,
            Rounds = package.Rounds.Select(r => r.ToRound()).ToArray(),
            Size = package.Size,
            TagIds = enriched.TagIds,
        };
    }

    private string BuildContentUri(PackageModel package) =>
        $"{_options.PublicUri}api/v1/packages/{package.Id}/download?callbackUri={Uri.EscapeDataString(_options.PackageUri + package.FileName)}";

    private IQueryable<EnrichedPackage> EnrichPackageQuery(IQueryable<PackageModel> packages) =>
        from p in packages
        join pt in connection.PackageTags on p.Id equals pt.PackageId into tags
        join pa in connection.PackageAuthors on p.Id equals pa.PackageId into authors
        join pr in connection.PackageRestrictions on p.Id equals pr.PackageId into restrictions
        select new EnrichedPackage(
            p,
            tags.DefaultIfEmpty().Select(t => t!.TagId).ToArray(),
            authors.DefaultIfEmpty().Select(a => a!.AuthorId).ToArray(),
            restrictions.DefaultIfEmpty().Select(r => r!.RestrictionId).ToArray());

    private static Expression<Func<PackageModel, bool>> BuildDifficultyPredicate(NumericFilter<short> difficultyFilter)
    {
        var compareMode = difficultyFilter.CompareMode;

        var param = Expression.Parameter(typeof(PackageModel));
        var val = Expression.Constant(difficultyFilter.Value);

        var difficulty = Expression.Property(param, nameof(PackageModel.Difficulty));
        var difficultyValue = Expression.Property(difficulty, nameof(Nullable<short>.Value));

        // List to hold the expressions
        var expressions = new List<Expression>();

        // Add the expressions based on the CompareMode
        if ((compareMode & CompareMode.LessThan) != 0)
        {
            expressions.Add(Expression.LessThan(difficultyValue, val));
        }

        if ((compareMode & CompareMode.EqualTo) != 0)
        {
            expressions.Add(Expression.Equal(difficultyValue, val));
        }

        if ((compareMode & CompareMode.GreaterThan) != 0)
        {
            expressions.Add(Expression.GreaterThan(difficultyValue, val));
        }

        // Combine all the expressions with OrElse
        var combinedExpression = expressions.Aggregate(Expression.OrElse);

        return Expression.Lambda<Func<PackageModel, bool>>(combinedExpression, param);
    }

    public async Task AddPackageAsync(
        Guid packageId,
        string packageName,
        PackageInfo packageMetadata,
        string packageFileName,
        long packageFileSize, 
        string? logoUri,
        CancellationToken cancellationToken)
    {
        var languageId = await InsertLanguageAsync(packageMetadata.Language == "en-US" ? "en-US" : "ru-RU" , cancellationToken);
        
        int? publisherId = string.IsNullOrEmpty(packageMetadata.Publisher)
            ? null
            : await InsertPublisherAsync(packageMetadata.Publisher ?? "", cancellationToken);

        await connection.Packages.InsertAsync(
            () => new PackageModel
            {
                Id = packageId,
                ContentTypeStatistic = packageMetadata.ContentTypeStatistic,
                FileName = packageFileName,
                OriginalFileName = packageName,
                CreateDate = packageMetadata.CreateDate,
                Difficulty = packageMetadata.Difficulty,
                Downloadable = true,
                DownloadCount = 0,
                LanguageId = languageId,
                LogoUri = logoUri,
                Name = packageMetadata.Name,
                PublisherId = publisherId,
                QuestionCount= packageMetadata.QuestionCount,
                Rounds = packageMetadata.Rounds,
                Size = packageFileSize
            },
            token: cancellationToken);

        foreach (var author in packageMetadata.Authors)
        {
            var authorId = await InsertAuthorAsync(author, cancellationToken);

            await connection.PackageAuthors.InsertAsync(() =>
                new PackageAuthor
                {
                    PackageId = packageId,
                    AuthorId = authorId
                },
                cancellationToken);
        }

        foreach (var tag in packageMetadata.Tags)
        {
            var tagId = await InsertTagAsync(tag, cancellationToken);

            await connection.PackageTags.InsertAsync(() =>
                new PackageTag
                {
                    PackageId = packageId,
                    TagId = tagId
                },
                cancellationToken);
        }

        if (string.IsNullOrWhiteSpace(packageMetadata.Restriction))
        {
            return;
        }

        var restrictionId = await InsertRestrictionAsync(WellKnownRestrictionNames.Age, packageMetadata.Restriction, cancellationToken);

        await connection.PackageRestrictions.InsertAsync(() =>
            new PackageRestriction
            {
                PackageId = packageId,
                RestrictionId = restrictionId
            },
            cancellationToken);
    }

    public async Task UpdatePackageAsync(
        Guid packageId,
        string packageName,
        PackageInfo packageMetadata,
        string packageFileName,
        long packageFileSize,
        string? logoUri,
        CancellationToken cancellationToken)
    {
        var languageId = await InsertLanguageAsync(packageMetadata.Language == "en-US" ? "en-US" : "ru-RU", cancellationToken);

        int? publisherId = string.IsNullOrEmpty(packageMetadata.Publisher)
            ? null
            : await InsertPublisherAsync(packageMetadata.Publisher ?? "", cancellationToken);

        await connection.Packages
            .Where(p => p.Id == packageId)
            .Set(p => p.ContentTypeStatistic, packageMetadata.ContentTypeStatistic)
            .Set(p => p.FileName, packageFileName)
            .Set(p => p.OriginalFileName, packageName)
            .Set(p => p.CreateDate, packageMetadata.CreateDate)
            .Set(p => p.Difficulty, packageMetadata.Difficulty)
            .Set(p => p.LanguageId, languageId)
            .Set(p => p.LogoUri, logoUri)
            .Set(p => p.Name, packageMetadata.Name)
            .Set(p => p.PublisherId, publisherId)
            .Set(p => p.QuestionCount, packageMetadata.QuestionCount)
            .Set(p => p.Rounds, packageMetadata.Rounds)
            .Set(p => p.Size, packageFileSize)
            .UpdateAsync(cancellationToken);

        await connection.PackageAuthors
            .Where(pa => pa.PackageId == packageId)
            .DeleteAsync(cancellationToken);

        foreach (var author in packageMetadata.Authors)
        {
            var authorId = await InsertAuthorAsync(author, cancellationToken);
            
            await connection.PackageAuthors.InsertAsync(() =>
                new PackageAuthor
                {
                    PackageId = packageId,
                    AuthorId = authorId
                },
                cancellationToken);
        }

        await connection.PackageTags
            .Where(pt => pt.PackageId == packageId)
            .DeleteAsync(cancellationToken);

        foreach (var tag in packageMetadata.Tags)
        {
            var tagId = await InsertTagAsync(tag, cancellationToken);
            
            await connection.PackageTags.InsertAsync(() =>
                new PackageTag
                {
                    PackageId = packageId,
                    TagId = tagId
                },
                cancellationToken);
        }

        await connection.PackageRestrictions
            .Where(pr => pr.PackageId == packageId)
            .DeleteAsync(cancellationToken);

        if (!string.IsNullOrWhiteSpace(packageMetadata.Restriction))
        {
            var restrictionId = await InsertRestrictionAsync(WellKnownRestrictionNames.Age, packageMetadata.Restriction, cancellationToken);
            
            await connection.PackageRestrictions.InsertAsync(() =>
                new PackageRestriction
                {
                    PackageId = packageId,
                    RestrictionId = restrictionId
                },
                cancellationToken);
        }
    }

    private async Task<int> InsertLanguageAsync(string languageCode, CancellationToken cancellationToken)
    {
        await connection.Languages.InsertOrUpdateAsync(
            () => new LanguageModel
            {
                Code = languageCode
            },
            null,
            () => new LanguageModel
            {
                Code = languageCode
            },
            cancellationToken);

        return (await connection.Languages.FirstAsync(a => a.Code == languageCode, token: cancellationToken)).Id;
    }

    private async Task<int> InsertPublisherAsync(string publisherName, CancellationToken cancellationToken)
    {
        await connection.Publishers.InsertOrUpdateAsync(
            () => new PublisherModel
            {
                Name = publisherName
            },
            null,
            () => new PublisherModel
            {
                Name = publisherName
            },
            cancellationToken);

        return (await connection.Publishers.FirstAsync(a => a.Name == publisherName, token: cancellationToken)).Id;
    }

    private async Task<int> InsertAuthorAsync(string authorName, CancellationToken cancellationToken)
    {
        await connection.Authors.InsertOrUpdateAsync(
            () => new AuthorModel
            {
                Name = authorName
            },
            null,
            () => new AuthorModel
            {
                Name = authorName
            },
            cancellationToken);

        return (await connection.Authors.FirstAsync(a => a.Name == authorName, token: cancellationToken)).Id;
    }

    private async Task<int> InsertTagAsync(string tagName, CancellationToken cancellationToken)
    {
        await connection.Tags.InsertOrUpdateAsync(
            () => new TagModel
            {
                Name = tagName
            },
            null,
            () => new TagModel
            {
                Name = tagName
            },
            cancellationToken);

        return (await connection.Tags.FirstAsync(t => t.Name == tagName, token: cancellationToken)).Id;
    }

    private async Task<int> InsertRestrictionAsync(string name, string value, CancellationToken cancellationToken)
    {
        await connection.Restrictions.InsertOrUpdateAsync(
            () => new RestrictionModel
            {
                Name = name,
                Value = value,
            },
            null,
            () => new RestrictionModel
            {
                Name = name,
                Value = value,
            },
            cancellationToken);

        return (await connection.Restrictions.FirstAsync(r => r.Name == name && r.Value == value, token: cancellationToken)).Id;
    }

    public async Task<Package> GetRandomPackageAsync(RandomPackageParameters randomPackageParameters, CancellationToken cancellationToken = default)
    {
        IQueryable<PackageModel> packages = connection.Packages;

        packages = BuildTagFilter(randomPackageParameters.TagIds, packages);
        packages = BuildRestrictionFilter(randomPackageParameters.RestrictionIds, packages);
        packages = BuildDifficultyFilter(randomPackageParameters.Difficulty, packages);
        packages = BuildLanguageFilter(randomPackageParameters.LanguageId, packages);

        packages = packages.OrderBy(p => Random()).Take(RandomPackageSourceCount);

        var sourcePackages = await packages.Select(p => new PackageMetadata(p.Id, p.Rounds)).ToArrayAsync(cancellationToken);

        if (sourcePackages.Length == 0)
        {
            throw new ServiceException(WellKnownSIStorageServiceErrorCode.PackageNotFound, HttpStatusCode.NotFound);
        }

        if (sourcePackages.Length == 1)
        {
            return await GetPackageAsync(sourcePackages[0].Id, cancellationToken);
        }

        var tags = randomPackageParameters.TagIds.IsNullOrEmpty()
            ? []
            : await connection.Tags.Where(t => randomPackageParameters.TagIds.Contains(t.Id)).Select(t => t.Name).ToArrayAsync(cancellationToken);

        var restrictions = randomPackageParameters.RestrictionIds.IsNullOrEmpty()
            ? []
            : await connection.Restrictions.Where(r => randomPackageParameters.RestrictionIds.Contains(r.Id)).Select(r => r.Value).ToArrayAsync(cancellationToken);

        var language = randomPackageParameters.LanguageId == null
            ? null
            : await connection.Languages.Where(l => l.Id == randomPackageParameters.LanguageId).Select(l => l.Code).FirstOrDefaultAsync(cancellationToken);

        var packageId = Guid.NewGuid();
        var filePath = tempPackagesService.GenerateFilePath(packageId);
        var fileName = Path.GetFileName(filePath);

        var generatorParameters = new PackageGeneratorParameters(
            Math.Max(1, Math.Min(5, randomPackageParameters.RoundCount)),
            Math.Max(1, Math.Min(10, randomPackageParameters.TableThemeCount)),
            Math.Max(1, Math.Min(10, randomPackageParameters.ThemeListThemeCount)),
            Math.Max(1, Math.Min(1000, randomPackageParameters.BaseQuestionPrice)));

        using (var fileStream = File.Create(filePath))
        using (var document = await RandomPackageGenerator.GeneratePackageAsync(fileStream, this, sourcePackages, generatorParameters, cancellationToken))
        {
            document.Package.Tags.AddRange(tags);
            document.Package.Restriction = string.Join(", ", restrictions);

            if (language != null)
            {
                document.Package.Language = language;
            }

            if (randomPackageParameters.Difficulty.HasValue)
            {
                document.Package.Difficulty = randomPackageParameters.Difficulty.Value;
            }

            document.Save();
        }

        return new Package
        {
            Id = packageId,
            ContentUri = new Uri(_options.TempUri + fileName, UriKind.Absolute),
            DirectContentUri = new Uri(_options.TempUri + fileName, UriKind.Absolute),
            CreateDate = DateOnly.FromDateTime(DateTime.UtcNow),
            DownloadCount = 0,
            Size = new FileInfo(filePath).Length,
            TagIds = randomPackageParameters.TagIds ?? Array.Empty<int>(),
            Difficulty = randomPackageParameters.Difficulty,
            RestrictionIds = randomPackageParameters.RestrictionIds ?? Array.Empty<int>(),
            LanguageId = randomPackageParameters.LanguageId
        };
    }

    private static IQueryable<PackageModel> BuildDifficultyFilter(short? difficulty, IQueryable<PackageModel> packages) =>
        difficulty.HasValue ? packages.Where(p => p.Difficulty == difficulty) : packages;

    public Task<SIPackages.SIDocument> GetPackageAsync(string packageId, CancellationToken cancellationToken = default)
    {
        var packagesFolder = Path.Combine(StringHelper.BuildRootedPath(_options.ContentFolder), PackagesFolder);
        var packagesFile = Path.ChangeExtension(Path.Combine(packagesFolder, packageId), "siq");

        if (!File.Exists(packagesFile))
        {
            logger.LogWarning("Cannot find file {fileName}", packagesFile);
            throw new ServiceException(WellKnownSIStorageServiceErrorCode.PackageNotFound, HttpStatusCode.InternalServerError);
        }

        return Task.FromResult(SIPackages.SIDocument.Load(File.OpenRead(packagesFile)));
    }

    private sealed record EnrichedPackage(PackageModel Package, int[] TagIds, int[] AuthorIds, int[] RestrictionIds);

    [Function("random", ServerSideOnly = true, CanBeNull = false, IsPure = false)]
    private static Guid Random() => Guid.NewGuid();

    public async Task<bool> DeletePackageAsync(Guid packageId, CancellationToken cancellationToken)
    {
        await connection.PackageAuthors
            .Where(pa => pa.PackageId == packageId)
            .DeleteAsync(cancellationToken);

        await connection.PackageTags
            .Where(pt => pt.PackageId == packageId)
            .DeleteAsync(cancellationToken);

        await connection.PackageRestrictions
            .Where(pr => pr.PackageId == packageId)
            .DeleteAsync(cancellationToken);

        var recordCount = await connection.Packages
            .Where(p => p.Id == packageId)
            .DeleteAsync(cancellationToken);

        return recordCount > 0;
    }
}
