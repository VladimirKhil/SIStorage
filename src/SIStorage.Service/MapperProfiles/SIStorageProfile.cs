using AutoMapper;
using SIStorage.Database.Models;
using SIStorage.Service.Contract.Models;

namespace SIStorage.Service.MapperProfiles;

/// <summary>
/// Defines a SIStorage mapping profile.
/// </summary>
internal sealed class SIStorageProfile : Profile
{
    public const string OptionsKey = "options";

    public SIStorageProfile()
    {
        CreateMap<TagModel, Tag>();
        CreateMap<RestrictionModel, Restriction>();
        CreateMap<AuthorModel, Author>();
        CreateMap<LanguageModel, Language>();
        CreateMap<PublisherModel, Publisher>();
        CreateMap<RoundModel, Round>();
    }
}
