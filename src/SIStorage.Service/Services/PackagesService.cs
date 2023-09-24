using AutoMapper;
using LinqToDB;
using Microsoft.Extensions.Options;
using SIStorage.Database;
using SIStorage.Database.Models;
using SIStorage.Service.Configuration;
using SIStorage.Service.Contract.Exceptions;
using SIStorage.Service.Contract.Models;
using SIStorage.Service.Contract.Requests;
using SIStorage.Service.Contract.Responses;
using SIStorage.Service.Contracts;
using System.Linq.Expressions;

namespace SIStorage.Service.Services;

/// <inheritdoc cref="IExtendedPackagesApi" />
internal sealed class PackagesService : IExtendedPackagesApi
{
    private readonly SIStorageDbConnection _siStorageDbConnection;
    private readonly SIStorageOptions _options;
    private readonly IMapper _mapper;
    private readonly ILogger<PackagesService> _logger;

    public PackagesService(
        SIStorageDbConnection siStorageDbConnection,
        IOptions<SIStorageOptions> options,
        IMapper mapper,
        ILogger<PackagesService> logger)
    {
        _siStorageDbConnection = siStorageDbConnection;
        _options = options.Value;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Package> GetPackageAsync(Guid packageId, CancellationToken cancellationToken = default)
    {
        var query = EnrichPackageQuery(_siStorageDbConnection.Packages.Where(p => p.Id == packageId));
        var package = await query.FirstOrDefaultAsync(cancellationToken) ?? throw new PackageNotFoundException();

        return ToPackage(package);
    }

    public async void IncrementDownloadCount(Guid packageId, CancellationToken cancellationToken = default)
    {
        try
        {
            await _siStorageDbConnection.Packages
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
        IQueryable<PackageModel> packages = _siStorageDbConnection.Packages;

        if (packageFilters.TagIds != null && packageFilters.TagIds.Any())
        {
            packages = packages.Where(
                p => _siStorageDbConnection.PackageTags.Any(
                    pt => pt.PackageId == p.Id && packageFilters.TagIds.Contains(pt.TagId)));
        }

        if (packageFilters.Difficulty != null)
        {
            packages = packages.Where(BuildDifficultyPredicate(packageFilters.Difficulty));
        }

        if (packageFilters.RestrictionIds != null)
        {
            packages = packages.Where(
                p => _siStorageDbConnection.PackageRestrictions.Any(
                    pr => pr.PackageId == p.Id && packageFilters.RestrictionIds.Contains(pr.RestrictionId)));
        }

        if (packageFilters.AuthorId != null)
        {
            packages = packages.Where(
                p => _siStorageDbConnection.PackageAuthors.Any(
                    pa => pa.PackageId == p.Id && packageFilters.AuthorId == pa.AuthorId));
        }

        if (packageFilters.PublisherId.HasValue)
        {
            packages = packages.Where(p => p.PublisherId == packageFilters.PublisherId);
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
            AtomTypesStatistic = package.AtomTypesStatistic,
            AuthorIds = enriched.AuthorIds,
            ContentUri = new Uri(BuildContentUri(package), UriKind.Absolute),
            CreateDate = package.CreateDate,
            Difficulty = package.Difficulty,
            DownloadCount = package.DownloadCount,
            ExtraInfo = null,
            Id = package.Id,
            Rating = 0, // TODO
            LanguageId = package.LanguageId,
            LogoUri = package.LogoUri == null
            ? null
            : new Uri($"{_options.LogoUri}{package.LogoUri}", UriKind.Absolute),
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
        $"{_options.DownloadCallbackUri}api/v1/packages/{package.Id}/download?callbackUri={Uri.EscapeDataString(_options.PackageUri + package.FileName)}";

    private IQueryable<EnrichedPackage> EnrichPackageQuery(IQueryable<PackageModel> packages) =>
        from p in packages
        join pt in _siStorageDbConnection.PackageTags on p.Id equals pt.PackageId into tags
        join pa in _siStorageDbConnection.PackageAuthors on p.Id equals pa.PackageId into authors
        join pr in _siStorageDbConnection.PackageRestrictions on p.Id equals pr.PackageId into restrictions
        select new EnrichedPackage(
            p,
            tags.DefaultIfEmpty().Select(t => t.TagId).ToArray(),
            authors.DefaultIfEmpty().Select(a => a.AuthorId).ToArray(),
            restrictions.DefaultIfEmpty().Select(r => r.RestrictionId).ToArray());

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

    private sealed record EnrichedPackage(PackageModel Package, int[] TagIds, int[] AuthorIds, int[] RestrictionIds);
}
