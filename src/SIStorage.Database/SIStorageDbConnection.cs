using LinqToDB;
using LinqToDB.Data;
using SIStorage.Database.Models;

namespace SIStorage.Database;

/// <summary>
/// Defines a database context.
/// </summary>
public sealed class SIStorageDbConnection(DataOptions options) : DataConnection(options)
{
    /// <summary>
    /// Package authors.
    /// </summary>
    public ITable<AuthorModel> Authors => this.GetTable<AuthorModel>();

    /// <summary>
    /// Package publishers.
    /// </summary>
    public ITable<PublisherModel> Publishers => this.GetTable<PublisherModel>();

    /// <summary>
    /// Package tags.
    /// </summary>
    public ITable<TagModel> Tags => this.GetTable<TagModel>();

    /// <summary>
    /// Package restrictions.
    /// </summary>
    public ITable<RestrictionModel> Restrictions => this.GetTable<RestrictionModel>();

    /// <summary>
    /// Package languages.
    /// </summary>
    public ITable<LanguageModel> Languages => this.GetTable<LanguageModel>();

    /// <summary>
    /// Packages.
    /// </summary>
    public ITable<PackageModel> Packages => this.GetTable<PackageModel>();

    /// <summary>
    /// Package - tag relation.
    /// </summary>
    public ITable<PackageTag> PackageTags => this.GetTable<PackageTag>();

    /// <summary>
    /// Package - author relation.
    /// </summary>
    public ITable<PackageAuthor> PackageAuthors => this.GetTable<PackageAuthor>();

    /// <summary>
    /// Package - restriction relation.
    /// </summary>
    public ITable<PackageRestriction> PackageRestrictions => this.GetTable<PackageRestriction>();
}
