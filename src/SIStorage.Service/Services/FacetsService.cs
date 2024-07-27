using LinqToDB;
using SIStorage.Database;
using SIStorage.Database.Models;
using SIStorage.Service.Contract;
using SIStorage.Service.Contract.Models;
using SIStorage.Service.Helpers;

namespace SIStorage.Service.Services;

internal sealed class FacetsService(SIStorageDbConnection siStorageDbConnection) : IFacetsApi
{
    public async Task<Publisher[]> GetPublishersAsync(int? languageId = null, CancellationToken cancellationToken = default)
    {
        IQueryable<PublisherModel> publishers;

        if (languageId.HasValue)
        {
            publishers = from p in siStorageDbConnection.Publishers
                         where siStorageDbConnection.Packages.Any(package =>
                            package.PublisherId == p.Id && package.LanguageId == languageId)
                         select p;
        }
        else
        {
            publishers = from p in siStorageDbConnection.Publishers
                         where siStorageDbConnection.Packages.Any(package => package.PublisherId == p.Id)
                         select p;
        }

        return await publishers.Select(p => p.ToPublisher()).ToArrayAsync(cancellationToken);
    }

    public async Task<Author[]> GetAuthorsAsync(int? languageId = null, CancellationToken cancellationToken = default)
    {
        IQueryable<AuthorModel> authors;

        if (languageId.HasValue)
        {
            authors = from a in siStorageDbConnection.Authors
                   where siStorageDbConnection.PackageAuthors.Any(pa =>
                        pa.AuthorId == a.Id
                        && siStorageDbConnection.Packages.Any(p => p.Id == pa.PackageId && p.LanguageId == languageId.Value))
                   select a;
        }
        else
        {
            authors = from a in siStorageDbConnection.Authors
                      where siStorageDbConnection.PackageAuthors.Any(pa => pa.AuthorId == a.Id)
                      select a;
        }

        return await authors.Select(a => a.ToAuthor()).ToArrayAsync(cancellationToken);
    }

    public async Task<Tag[]> GetTagsAsync(int? languageId = null, CancellationToken cancellationToken = default)
    {
        IQueryable<TagModel> tags;

        if (languageId.HasValue)
        {
            tags = from t in siStorageDbConnection.Tags
                   where siStorageDbConnection.PackageTags.Any(pt =>
                        pt.TagId == t.Id
                        && siStorageDbConnection.Packages.Any(p => p.Id == pt.PackageId && p.LanguageId == languageId.Value))
                   select t;
        }
        else
        {
            tags = from t in siStorageDbConnection.Tags
                   where siStorageDbConnection.PackageTags.Any(pt => pt.TagId == t.Id)
                   select t;
        }

        return await tags.Select(t => t.ToTag()).ToArrayAsync(cancellationToken);
    }

    public async Task<Restriction[]> GetRestrictionsAsync(CancellationToken cancellationToken = default)
    {
        var restrictions = from r in siStorageDbConnection.Restrictions
                           where siStorageDbConnection.PackageRestrictions.Any(pr => pr.RestrictionId == r.Id)
                           select r;

        return await restrictions.Select(r => r.ToRestriction()).ToArrayAsync(cancellationToken);
    }

    public async Task<Language[]> GetLanguagesAsync(CancellationToken cancellationToken = default)
    {
        var languages = from l in siStorageDbConnection.Languages
                        where siStorageDbConnection.Packages.Any(p => p.LanguageId == l.Id)
                        select l;

        return await languages.Select(l => l.ToLanguage()).ToArrayAsync(cancellationToken);
    }
}
