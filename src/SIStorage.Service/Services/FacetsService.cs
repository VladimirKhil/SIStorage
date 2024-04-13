using AutoMapper;
using LinqToDB;
using SIStorage.Database;
using SIStorage.Database.Models;
using SIStorage.Service.Contract;
using SIStorage.Service.Contract.Models;

namespace SIStorage.Service.Services;

internal sealed class FacetsService : IFacetsApi
{
    private readonly SIStorageDbConnection _siStorageDbConnection;
    private readonly IMapper _mapper;

    public FacetsService(SIStorageDbConnection siStorageDbConnection, IMapper mapper)
    {
        _siStorageDbConnection = siStorageDbConnection;
        _mapper = mapper;
    }

    public async Task<Publisher[]> GetPublishersAsync(int? languageId = null, CancellationToken cancellationToken = default)
    {
        IQueryable<PublisherModel> publishers;

        if (languageId.HasValue)
        {
            publishers = from p in _siStorageDbConnection.Publishers
                         where _siStorageDbConnection.Packages.Any(package =>
                            package.PublisherId == p.Id && package.LanguageId == languageId)
                         select p;
        }
        else
        {
            publishers = from p in _siStorageDbConnection.Publishers
                         where _siStorageDbConnection.Packages.Any(package => package.PublisherId == p.Id)
                         select p;
        }

        return _mapper.Map<Publisher[]>(await publishers.ToArrayAsync(cancellationToken));
    }

    public async Task<Author[]> GetAuthorsAsync(int? languageId = null, CancellationToken cancellationToken = default)
    {
        IQueryable<AuthorModel> authors;

        if (languageId.HasValue)
        {
            authors = from a in _siStorageDbConnection.Authors
                   where _siStorageDbConnection.PackageAuthors.Any(pa =>
                        pa.AuthorId == a.Id
                        && _siStorageDbConnection.Packages.Any(p => p.Id == pa.PackageId && p.LanguageId == languageId.Value))
                   select a;
        }
        else
        {
            authors = from a in _siStorageDbConnection.Authors
                      where _siStorageDbConnection.PackageAuthors.Any(pa => pa.AuthorId == a.Id)
                      select a;
        }

        return _mapper.Map<Author[]>(await authors.ToArrayAsync(cancellationToken));
    }

    public async Task<Tag[]> GetTagsAsync(int? languageId = null, CancellationToken cancellationToken = default)
    {
        IQueryable<TagModel> tags;

        if (languageId.HasValue)
        {
            tags = from t in _siStorageDbConnection.Tags
                   where _siStorageDbConnection.PackageTags.Any(pt =>
                        pt.TagId == t.Id
                        && _siStorageDbConnection.Packages.Any(p => p.Id == pt.PackageId && p.LanguageId == languageId.Value))
                   select t;
        }
        else
        {
            tags = from t in _siStorageDbConnection.Tags
                   where _siStorageDbConnection.PackageTags.Any(pt => pt.TagId == t.Id)
                   select t;
        }

        return _mapper.Map<Tag[]>(await tags.ToArrayAsync(cancellationToken));
    }

    public async Task<Restriction[]> GetRestrictionsAsync(CancellationToken cancellationToken = default)
    {
        var restrictions = from r in _siStorageDbConnection.Restrictions
                           where _siStorageDbConnection.PackageRestrictions.Any(pr => pr.RestrictionId == r.Id)
                           select r;

        return _mapper.Map<Restriction[]>(await restrictions.ToArrayAsync(cancellationToken));
    }

    public async Task<Language[]> GetLanguagesAsync(CancellationToken cancellationToken = default)
    {
        var languages = from l in _siStorageDbConnection.Languages
                        where _siStorageDbConnection.Packages.Any(p => p.LanguageId == l.Id)
                        select l;

        return _mapper.Map<Language[]>(await languages.ToArrayAsync(cancellationToken));
    }
}
