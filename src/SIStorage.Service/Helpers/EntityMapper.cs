using SIStorage.Database.Models;
using SIStorage.Service.Contract.Models;

namespace SIStorage.Service.Helpers;

internal static class EntityMapper
{
    internal static Tag ToTag(this TagModel model) => new(model.Id, model.Name);

    internal static Restriction ToRestriction(this RestrictionModel model) => new(model.Id, model.Name ?? "", model.Value ?? "");

    internal static Author ToAuthor(this AuthorModel model) => new(model.Id, model.Name ?? "");

    internal static Language ToLanguage(this LanguageModel model) => new(model.Id, model.Code ?? "");

    internal static Publisher ToPublisher(this PublisherModel model) => new(model.Id, model.Name ?? "");

    internal static Round ToRound(this RoundModel model) => new(model.Name, model.ThemeNames);
}
