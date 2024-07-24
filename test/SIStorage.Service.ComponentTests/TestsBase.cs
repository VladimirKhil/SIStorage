using AutoMapper;
using LinqToDB;
using LinqToDB.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using SIStorage.Database;
using SIStorage.Database.Models;
using SIStorage.Service.Configuration;
using SIStorage.Service.Contract;
using SIStorage.Service.Contract.Common;
using SIStorage.Service.Contracts;
using SIStorage.Service.MapperProfiles;
using SIStorage.Service.Services;
using System.Linq.Expressions;

namespace SIStorage.Service.ComponentTests;

/// <summary>
/// Provides base methods for SIStorage service component tests.
/// </summary>
/// <remarks>
/// SIStorage service and PostgreSQL must be started before tests to run.
/// This tests class clears the provided database, fills it with test data and then checks the SIStorage API.
/// </remarks>
public abstract class TestsBase
{
    protected IFacetsApi FacetsApi { get; }

    protected IExtendedPackagesApi PackagesApi { get; }

    protected SIStorageDbConnection DbConnection { get; }

    protected int CommonLanguageId { get; private set; }

    protected int SecondaryLanguageId { get; private set; }

    protected Guid Package1Id { get; private set; }

    protected Guid Package2Id { get; private set; }

    protected Guid Package3Id { get; private set; }

    protected Guid Package4Id { get; private set; }

    protected Guid Package5Id { get; private set; }

    public TestsBase()
    {
        var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
        var configuration = builder.Build();

        var services = new ServiceCollection();
        var mapperConfiguration = new MapperConfiguration(cfg => cfg.AddProfile<SIStorageProfile>());
        var mapper = mapperConfiguration.CreateMapper();

        services.AddSingleton(mapper);
        services.Configure<SIStorageOptions>(configuration.GetSection(SIStorageOptions.ConfigurationSectionName));
        services.AddSIStorageDatabase(configuration);
        services.AddSingleton<ILoggerFactory, NullLoggerFactory>();
        services.AddSingleton<ILogger<PackagesService>, NullLogger<PackagesService>>();
        services.AddScoped<IFacetsApi, FacetsService>();
        services.AddScoped<IExtendedPackagesApi, PackagesService>();
        services.AddSingleton<ITempPackagesService, TempPackagesService>();

        var serviceProvider = services.BuildServiceProvider();

        FacetsApi = serviceProvider.GetRequiredService<IFacetsApi>();
        PackagesApi = serviceProvider.GetRequiredService<IExtendedPackagesApi>();

        DbConnection = serviceProvider.GetRequiredService<SIStorageDbConnection>();
    }

    [OneTimeSetUp]
    public void PopulateTestData()
    {
        DbConnection.PackageAuthors.Truncate();
        DbConnection.PackageRestrictions.Truncate();
        DbConnection.PackageTags.Truncate();
        DbConnection.Packages.Delete();
        DbConnection.Authors.Delete();
        DbConnection.Publishers.Delete();
        DbConnection.Tags.Delete();
        DbConnection.Restrictions.Delete();
        DbConnection.Languages.Delete();

        ResetSequence("Tags_Id_seq");
        ResetSequence("Authors_Id_seq");
        ResetSequence("Restrictions_Id_seq");
        ResetSequence("Languages_Id_seq");
        ResetSequence("Publishers_Id_seq");

        DbConnection.Authors.Insert(() => new AuthorModel { Name = "author1" });
        DbConnection.Authors.Insert(() => new AuthorModel { Name = "author2" });
        DbConnection.Authors.Insert(() => new AuthorModel { Name = "author3" });

        var author1Id = DbConnection.Authors.First(a => a.Name == "author1").Id;
        var author2Id = DbConnection.Authors.First(a => a.Name == "author2").Id;
        var author3Id = DbConnection.Authors.First(a => a.Name == "author3").Id;

        DbConnection.Publishers.Insert(() => new PublisherModel { Name = "publisher1" });
        DbConnection.Publishers.Insert(() => new PublisherModel { Name = "publisher2" });
        DbConnection.Publishers.Insert(() => new PublisherModel { Name = "publisher3" });

        var publisher1Id = DbConnection.Publishers.First(p => p.Name == "publisher1").Id;
        var publisher2Id = DbConnection.Publishers.First(p => p.Name == "publisher2").Id;
        var publisher3Id = DbConnection.Publishers.First(p => p.Name == "publisher3").Id;

        DbConnection.Tags.Insert(() => new TagModel { Name = "tag1" });
        DbConnection.Tags.Insert(() => new TagModel { Name = "tag2" });
        DbConnection.Tags.Insert(() => new TagModel { Name = "tag3" });
        DbConnection.Tags.Insert(() => new TagModel { Name = "tag4" });
        DbConnection.Tags.Insert(() => new TagModel { Name = "tag5" });

        var tag1Id = DbConnection.Tags.First(t => t.Name == "tag1").Id;
        var tag2Id = DbConnection.Tags.First(t => t.Name == "tag2").Id;
        var tag3Id = DbConnection.Tags.First(t => t.Name == "tag3").Id;
        var tag4Id = DbConnection.Tags.First(t => t.Name == "tag4").Id;

        DbConnection.Languages.Insert(() => new LanguageModel { Code = "language1" });
        DbConnection.Languages.Insert(() => new LanguageModel { Code = "language2" });
        DbConnection.Languages.Insert(() => new LanguageModel { Code = "language3" });

        CommonLanguageId = DbConnection.Languages.First(l => l.Code == "language1").Id;
        SecondaryLanguageId = DbConnection.Languages.First(l => l.Code == "language2").Id;

        DbConnection.Restrictions.Insert(() => new RestrictionModel
        {
            Name = WellKnownRestrictionNames.Age,
            Value = WellKnownRestrictionValues.Age12Plus
        });

        DbConnection.Restrictions.Insert(() => new RestrictionModel
        {
            Name = WellKnownRestrictionNames.Age,
            Value = WellKnownRestrictionValues.Age18Plus
        });

        DbConnection.Restrictions.Insert(() => new RestrictionModel
        {
            Name = WellKnownRestrictionNames.Age,
            Value = "custom value"
        });

        var restriction12 = DbConnection.Restrictions.First(r => r.Value == WellKnownRestrictionValues.Age12Plus).Id;
        var restriction18 = DbConnection.Restrictions.First(r => r.Value == WellKnownRestrictionValues.Age18Plus).Id;

        Package1Id = Guid.NewGuid();
        Package2Id = Guid.NewGuid();
        Package3Id = Guid.NewGuid();
        Package4Id = Guid.NewGuid();
        Package5Id = Guid.NewGuid();

        DbConnection.Packages.Insert(CreatePackageModel(Package1Id, CommonLanguageId, publisher1Id));
        DbConnection.PackageRestrictions.Insert(() => new PackageRestriction { PackageId = Package1Id, RestrictionId = restriction12 });
        DbConnection.PackageAuthors.Insert(() => new PackageAuthor { PackageId = Package1Id, AuthorId = author1Id });
        DbConnection.PackageTags.Insert(() => new PackageTag { PackageId = Package1Id, TagId = tag1Id });
        DbConnection.PackageTags.Insert(() => new PackageTag { PackageId = Package1Id, TagId = tag2Id });

        DbConnection.Packages.Insert(CreatePackageModel(Package2Id, CommonLanguageId, publisher1Id));
        DbConnection.PackageTags.Insert(() => new PackageTag { PackageId = Package2Id, TagId = tag1Id });
        DbConnection.PackageTags.Insert(() => new PackageTag { PackageId = Package2Id, TagId = tag2Id });
        DbConnection.PackageTags.Insert(() => new PackageTag { PackageId = Package2Id, TagId = tag3Id });

        DbConnection.Packages.Insert(CreatePackageModel(Package3Id, CommonLanguageId, publisher1Id));
        DbConnection.PackageTags.Insert(() => new PackageTag { PackageId = Package3Id, TagId = tag1Id });
        DbConnection.PackageRestrictions.Insert(() => new PackageRestriction { PackageId = Package3Id, RestrictionId = restriction18 });

        DbConnection.Packages.Insert(CreatePackageModel(Package4Id, CommonLanguageId, publisher2Id));
        DbConnection.PackageAuthors.Insert(() => new PackageAuthor { PackageId = Package4Id, AuthorId = author2Id });

        DbConnection.Packages.Insert(CreatePackageModel(Package5Id, SecondaryLanguageId, publisher3Id));
        DbConnection.PackageTags.Insert(() => new PackageTag { PackageId = Package5Id, TagId = tag4Id });
        DbConnection.PackageAuthors.Insert(() => new PackageAuthor { PackageId = Package5Id, AuthorId = author3Id });
    }

    [OneTimeTearDown]
    public void Cleanup() => DbConnection.Dispose();

    private void ResetSequence(string sequenceName) => DbConnection.Execute(
        $"""
        do $$
        begin
          IF EXISTS (SELECT 0 FROM pg_class where relname = '{sequenceName}') THEN
            ALTER SEQUENCE "sistorage"."{sequenceName}" RESTART WITH 1;
          END if;
        end;
        $$
        """);

    private static Expression<Func<PackageModel>> CreatePackageModel(Guid id, int languageId, int publisherId) => () =>
        new PackageModel
        {
            Id = id,
            Name = "name1",
            CreateDate = new DateOnly(2000, 1, 1),
            Difficulty = 5,
            LanguageId = languageId,
            PublisherId = publisherId,
            OriginalFileName = "testuri.siq",
            FileName = "file-name",
            Downloadable = true,
            DownloadCount = 0,
            Size = 100,
            Rounds = new RoundModel[]
            {
                new("round1", new[] { "theme1", "theme2", "theme3" })
            },
            QuestionCount = 20,
            ContentTypeStatistic = new Dictionary<string, short>
            {
                { "image", 3 },
                { "audio", 2 }
            }
        };
}
