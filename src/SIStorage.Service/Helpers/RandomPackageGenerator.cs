using SIPackages;
using SIPackages.Core;
using SIStorage.Database.Models;
using SIStorage.Service.Contracts;
using SIStorage.Service.Models;
using System.Text;

namespace SIStorage.Service.Helpers;

/// <summary>
/// Generates random packages from existing ones.
/// </summary>
internal static class RandomPackageGenerator
{
    /// <summary>
    /// Random data marker.
    /// </summary>
    public const string RandomIndicator = "@{random}";

    /// <summary>
    /// Round marker.
    /// </summary>
    public const string RoundIndicator = "@{round}";

    /// <summary>
    /// Generates a random package.
    /// </summary>
    /// <param name="targetStream">Stream that will contain package data.</param>
    /// <param name="provider">Packages provider.</param>
    /// <param name="packages">Source packages.</param>
    /// <param name="parameters">Generator parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Generated document.</returns>
    internal static async Task<SIDocument> GeneratePackageAsync(
        Stream targetStream,
        IPackagesProvider provider,
        IReadOnlyList<PackageMetadata> packages,
        PackageGeneratorParameters parameters,
        CancellationToken cancellationToken)
    {
        var allThemeNames = new HashSet<string>();
        var retryCounter = 10;

        var packageCache = new Dictionary<Guid, SIDocument>();

        var doc = SIDocument.Create(RandomIndicator, RandomIndicator, targetStream);
        var packageComments = new StringBuilder(RandomIndicator);

        try
        {
            for (var i = 0; i < parameters.RoundCount; i++)
            {
                doc.Package.Rounds.Add(new Round { Type = RoundTypes.Standart, Name = (i + 1).ToString() });

                for (var j = 0; j < parameters.CommonThemeCount; j++)
                {
                    if (!await ExtractThemeAsync(
                        provider,
                        packageCache,
                        doc,
                        packages,
                        allThemeNames,
                        parameters,
                        i,
                        package => package.Rounds[..^1],
                        packageComments,
                        cancellationToken))
                    {
                        if (retryCounter-- >= 0)
                        {
                            j--;
                        }

                        continue;
                    }
                }
            }

            doc.Package.Rounds.Add(new Round { Type = RoundTypes.Final, Name = (parameters.RoundCount + 1).ToString() });

            for (var j = 0; j < parameters.FinalThemeCount; j++)
            {
                if (!await ExtractThemeAsync(
                    provider,
                    packageCache,
                    doc,
                    packages,
                    allThemeNames,
                    parameters,
                    parameters.RoundCount,
                    package => package.Rounds[^1..],
                    packageComments,
                    cancellationToken))
                {
                    if (retryCounter-- >= 0)
                    {
                        j--;
                    }

                    continue;
                }
            }

            doc.Package.Info.Comments.Text += packageComments.ToString();
            doc.Upgrade();
        }
        finally
        {
            foreach (var package in packageCache.Values)
            {
                package.Dispose();
            }
        }

        return doc;
    }

    private static async Task<bool> ExtractThemeAsync(
        IPackagesProvider provider,
        Dictionary<Guid, SIDocument> packageCache,
        SIDocument targetDocument,
        IReadOnlyList<PackageMetadata> packages,
        HashSet<string> allThemeNames,
        PackageGeneratorParameters parameters,
        int targetRoundIndex,
        Func<PackageMetadata, RoundModel[]> roundSelector,
        StringBuilder packageComments,
        CancellationToken cancellationToken = default)
    {
        var packageIndex = Random.Shared.Next(packages.Count);
        var package = packages[packageIndex];
        var filteredRounds = roundSelector(package);

        if (filteredRounds.Length == 0)
        {
            return false;
        }

        var roundIndex = Random.Shared.Next(filteredRounds.Length);
        var roundMeta = package.Rounds[roundIndex];
        var themeIndex = Random.Shared.Next(roundMeta.ThemeNames.Length);
        var themeName = roundMeta.ThemeNames[themeIndex];

        if (allThemeNames.Contains(themeName))
        {
            return false;
        }

        allThemeNames.Add(themeName);

        if (!packageCache.TryGetValue(package.Id, out var document))
        {
            packageCache[package.Id] = document = await provider.GetPackageAsync(package.Id.ToString(), cancellationToken);
        }

        var round = document.Package.Rounds[roundIndex];
        var theme = round.Themes[themeIndex];

        InheritAuthors(document, round, theme);
        InheritSources(document, round, theme);

        for (var i = 0; i < theme.Questions.Count; i++)
        {
            theme.Questions[i].Price = (targetRoundIndex + 1) * (i + 1) * parameters.BaseQuestionPrice;

            InheritAuthors(document, theme.Questions[i]);
            InheritSources(document, theme.Questions[i]);
            await InheritContentAsync(targetDocument, document, theme.Questions[i], cancellationToken);
        }

        targetDocument.Package.Rounds[targetRoundIndex].Themes.Add(theme);
        packageComments.AppendFormat("{0}:{1}:{2};", package.Id, document.Package.Rounds.IndexOf(round), themeIndex);
        return true;
    }

    private static void InheritAuthors(SIDocument sourceDocument, Question question)
    {
        var authors = question.Info.Authors;

        if (authors.Count <= 0)
        {
            return;
        }

        for (int i = 0; i < authors.Count; i++)
        {
            var link = sourceDocument.GetLink(question.Info.Authors, i, out string tail);

            if (link != null)
            {
                question.Info.Authors[i] = link + tail;
            }
        }
    }

    private static void InheritSources(SIDocument sourceDocument, Question question)
    {
        var sources = question.Info.Sources;

        if (sources.Count <= 0)
        {
            return;
        }

        for (int i = 0; i < sources.Count; i++)
        {
            var link = sourceDocument.GetLink(question.Info.Sources, i, out string tail);

            if (link != null)
            {
                question.Info.Sources[i] = link + tail;
            }
        }
    }

    private static async Task InheritContentAsync(
        SIDocument targetDocument,
        SIDocument sourceDocument,
        Question question,
        CancellationToken cancellationToken = default)
    {
        foreach (var contentItem in question.GetContent())
        {
            if (contentItem.Type == ContentTypes.Text)
            {
                continue;
            }

            var mediaInfo = sourceDocument.TryGetMedia(contentItem);

            if (mediaInfo.HasValue && mediaInfo.Value.HasStream)
            {
                var collection = targetDocument.TryGetCollection(contentItem.Type);

                if (collection != null)
                {
                    using var stream = mediaInfo.Value.Stream!;
                    await collection.AddFileAsync(contentItem.Value, stream, cancellationToken);
                }
            }
        }
    }

    private static void InheritAuthors(SIDocument sourceDocument, Round round, Theme theme)
    {
        var authors = theme.Info.Authors;

        if (authors.Count == 0)
        {
            authors = round.Info.Authors;

            if (authors.Count == 0)
            {
                authors = sourceDocument.Package.Info.Authors;
            }

            if (authors.Count > 0)
            {
                var realAuthors = sourceDocument.GetRealAuthors(authors);
                theme.Info.Authors.Clear();

                foreach (var item in realAuthors)
                {
                    theme.Info.Authors.Add(item);
                }
            }
        }
        else
        {
            for (int i = 0; i < authors.Count; i++)
            {
                var link = sourceDocument.GetLink(theme.Info.Authors, i, out string tail);

                if (link != null)
                {
                    theme.Info.Authors[i] = link + tail;
                }
            }
        }
    }

    private static void InheritSources(SIDocument sourceDocument, Round round, Theme theme)
    {
        var sources = theme.Info.Sources;

        if (sources.Count == 0)
        {
            sources = round.Info.Sources;

            if (sources.Count == 0)
            {
                sources = sourceDocument.Package.Info.Sources;
            }

            if (sources.Count > 0)
            {
                var realSources = sourceDocument.GetRealSources(sources);
                theme.Info.Sources.Clear();

                foreach (var item in realSources)
                {
                    theme.Info.Sources.Add(item);
                }
            }
        }
        else
        {
            for (int i = 0; i < sources.Count; i++)
            {
                var link = sourceDocument.GetLink(theme.Info.Sources, i, out string tail);

                if (link != null)
                {
                    theme.Info.Sources[i] = link + tail;
                }
            }
        }
    }
}
