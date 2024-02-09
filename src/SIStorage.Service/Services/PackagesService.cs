using AutoMapper;
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
using SIStorage.Service.Models;
using System.Linq.Expressions;
using System.Net;

namespace SIStorage.Service.Services;

/// <inheritdoc cref="IExtendedPackagesApi" />
internal sealed class PackagesService : IExtendedPackagesApi
{
    private readonly SIStorageDbConnection _connection;
    private readonly SIStorageOptions _options;
    private readonly IMapper _mapper;
    private readonly ILogger<PackagesService> _logger;

    public PackagesService(
        SIStorageDbConnection connection,
        IOptions<SIStorageOptions> options,
        IMapper mapper,
        ILogger<PackagesService> logger)
    {
        _connection = connection;
        _options = options.Value;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Package> GetPackageAsync(Guid packageId, CancellationToken cancellationToken = default)
    {
        var query = EnrichPackageQuery(_connection.Packages.Where(p => p.Id == packageId));
        var package = await query.FirstOrDefaultAsync(cancellationToken) ?? throw new ServiceException(WellKnownSIStorageServiceErrorCode.PackageNotFound, HttpStatusCode.NotFound);

        return ToPackage(package);
    }

    public async Task IncrementDownloadCountAsync(Guid packageId, CancellationToken cancellationToken = default)
    {
        try
        {
            await _connection.Packages
                .Where(p => p.Id == packageId)
                .Set(p => p.DownloadCount, p => p.DownloadCount + 1)
                .UpdateAsync(cancellationToken);
        }
        catch (Exception exc)
        {
            _logger.LogWarning(exc, "Error while incrementing download counter for package {packageId}: {message}", packageId, exc.Message);
        }
    }

    public async Task<PackagesPage> GetPackagesAsync(
        PackageFilters packageFilters,
        PackageSelectionParameters packageSelectionParameters,
        CancellationToken cancellationToken = default)
    {
        IQueryable<PackageModel> packages = _connection.Packages;

        if (!packageFilters.TagIds.IsNullOrEmpty())
        {
            if (packageFilters.TagIds.Length == 1 && packageFilters.TagIds[0] == -1)
            {
                packages = packages.Where(p => !_connection.PackageTags.Any(pt => pt.PackageId == p.Id));
            }
            else
            {
                packages = packages.Where(
                    p => _connection.PackageTags.Any(
                        pt => pt.PackageId == p.Id && packageFilters.TagIds.Contains(pt.TagId)));
            }
        }

        if (packageFilters.Difficulty != null)
        {
            packages = packages.Where(BuildDifficultyPredicate(packageFilters.Difficulty));
        }

        if (!packageFilters.RestrictionIds.IsNullOrEmpty())
        {
            if (packageFilters.RestrictionIds.Length == 1 && packageFilters.RestrictionIds[0] == -1)
            {
                packages = packages.Where(p => !_connection.PackageRestrictions.Any(pr => pr.PackageId == p.Id));
            }
            else
            {
                packages = packages.Where(
                    p => _connection.PackageRestrictions.Any(
                        pr => pr.PackageId == p.Id && packageFilters.RestrictionIds.Contains(pr.RestrictionId)));
            }
        }

        if (packageFilters.AuthorId != null)
        {
            packages = packages.Where(
                p => _connection.PackageAuthors.Any(
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

        if (packageFilters.LanguageId.HasValue)
        {
            packages = packages.Where(p => p.LanguageId == packageFilters.LanguageId);
        }

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
            Rounds = _mapper.Map<Round[]>(package.Rounds),
            Size = package.Size,
            TagIds = enriched.TagIds,
        };
    }

    private string BuildContentUri(PackageModel package) =>
        $"{_options.PublicUri}api/v1/packages/{package.Id}/download?callbackUri={Uri.EscapeDataString(_options.PackageUri + package.FileName)}";

    private IQueryable<EnrichedPackage> EnrichPackageQuery(IQueryable<PackageModel> packages) =>
        from p in packages
        join pt in _connection.PackageTags on p.Id equals pt.PackageId into tags
        join pa in _connection.PackageAuthors on p.Id equals pa.PackageId into authors
        join pr in _connection.PackageRestrictions on p.Id equals pr.PackageId into restrictions
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

        await _connection.Packages.InsertAsync(
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

            await _connection.PackageAuthors.InsertAsync(() =>
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

            await _connection.PackageTags.InsertAsync(() =>
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

        await _connection.PackageRestrictions.InsertAsync(() =>
            new PackageRestriction
            {
                PackageId = packageId,
                RestrictionId = restrictionId
            },
            cancellationToken);
    }

    private async Task<int> InsertLanguageAsync(string languageCode, CancellationToken cancellationToken)
    {
        await _connection.Languages.InsertOrUpdateAsync(
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

        return (await _connection.Languages.FirstAsync(a => a.Code == languageCode, token: cancellationToken)).Id;
    }

    private async Task<int> InsertPublisherAsync(string publisherName, CancellationToken cancellationToken)
    {
        await _connection.Publishers.InsertOrUpdateAsync(
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

        return (await _connection.Publishers.FirstAsync(a => a.Name == publisherName, token: cancellationToken)).Id;
    }

    private async Task<int> InsertAuthorAsync(string authorName, CancellationToken cancellationToken)
    {
        await _connection.Authors.InsertOrUpdateAsync(
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

        return (await _connection.Authors.FirstAsync(a => a.Name == authorName, token: cancellationToken)).Id;
    }

    private async Task<int> InsertTagAsync(string tagName, CancellationToken cancellationToken)
    {
        await _connection.Tags.InsertOrUpdateAsync(
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

        return (await _connection.Tags.FirstAsync(t => t.Name == tagName, token: cancellationToken)).Id;
    }

    private async Task<int> InsertRestrictionAsync(string name, string value, CancellationToken cancellationToken)
    {
        await _connection.Restrictions.InsertOrUpdateAsync(
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

        return (await _connection.Restrictions.FirstAsync(r => r.Name == name && r.Value == value, token: cancellationToken)).Id;
    }

    public async Task<Package> GetRandomPackageAsync(RandomPackageParameters randomPackageParameters, CancellationToken cancellationToken = default)
    {
        // TODO: generate random package, save it to temporary folder accessible by NGinx and return a link to it

        return new Package
        {
            TagIds = randomPackageParameters.TagIds ?? Array.Empty<int>(),
            Difficulty = randomPackageParameters.Difficulty,
            RestrictionIds = randomPackageParameters.RestrictionIds ?? Array.Empty<int>(),
            LanguageId = randomPackageParameters.LanguageId
        };
    }

    public void CleanTempPackages()
    {
        // TODO: clean old packages from temporary folder
        throw new NotImplementedException();
    }

    private sealed record EnrichedPackage(PackageModel Package, int[] TagIds, int[] AuthorIds, int[] RestrictionIds);
}
