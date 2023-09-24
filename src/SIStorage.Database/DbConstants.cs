namespace SIStorage.Database;

/// <summary>
/// Provides well-known SIStorage database constants.
/// </summary>
public static class DbConstants
{
    public const string Schema = "sistorage";
    public const string DbName = "sistorage";

    public const string Publishers = nameof(Publishers);
    public const string Authors = nameof(Authors);
    public const string Tags = nameof(Tags);
    public const string Restrictions = nameof(Restrictions);
    public const string Languages = nameof(Languages);
    public const string Packages = nameof(Packages);

    public const string PackageTags = nameof(PackageTags);
    public const string PackageAuthors = nameof(PackageAuthors);
    public const string PackageRestrictions = nameof(PackageRestrictions);
}
