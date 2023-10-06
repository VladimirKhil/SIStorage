using SIPackages;
using SIPackages.Core;
using SIStorage.Database.Models;
using SIStorage.Service.Contracts;
using SIStorage.Service.Models;

namespace SIStorage.Service.Services;

public sealed class PackageIndexer : IPackageIndexer
{
    public PackageInfo IndexPackage(SIDocument document)
    {
        document.Upgrade();

        var contentTypeStatistic = new Dictionary<string, short>();
        var questionCount = 0;
        var rounds = new List<RoundModel>();

        foreach (var round in document.Package.Rounds)
        {
            rounds.Add(new RoundModel { Name = round.Name, ThemeNames = round.Themes.Select(t => t.Name).ToArray() });

            foreach (var theme in round.Themes)
            {
                questionCount += theme.Questions.Count;

                foreach (var question in theme.Questions)
                {
                    foreach (var content in question.GetContent())
                    {
                        if (content.Type == AtomTypes.Text)
                        {
                            continue;
                        }

                        contentTypeStatistic.TryGetValue(content.Type, out var count);
                        contentTypeStatistic[content.Type] = (short)(count + 1);
                    }
                }
            }
        }

        var language = document.Package.Language;
        var publisher = document.Package.Publisher;

        var authors = document.Package.Info.Authors.ToArray();
        var tags = document.Package.Tags.ToArray();
        var restriction = document.Package.Restriction;

        return new PackageInfo
        {
            CreateDate = DateOnly.FromDateTime(DateTime.UtcNow),
            Difficulty = (short)document.Package.Difficulty,
            Language = language,
            Name = document.Package.Name,
            Publisher = publisher,
            ContentTypeStatistic = contentTypeStatistic,
            QuestionCount = (short)questionCount,
            Rounds = rounds.ToArray(),
            Authors = authors,
            Tags = tags,
            Restriction = restriction
        };
    }
}
