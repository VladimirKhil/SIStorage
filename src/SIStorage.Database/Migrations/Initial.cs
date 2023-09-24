using FluentMigrator;
using SIStorage.Database.Models;

namespace SIStorage.Database.Migrations;

[Migration(202211260000, "Initial migration")]
public sealed class Initial : Migration
{
    public override void Up()
    {
        Create.Table(DbConstants.Tags)
            .WithColumn(nameof(TagModel.Id)).AsInt32().PrimaryKey().Identity()
            .WithColumn(nameof(TagModel.Name)).AsString().NotNullable().Unique();

        Create.Table(DbConstants.Authors)
            .WithColumn(nameof(AuthorModel.Id)).AsInt32().PrimaryKey().Identity()
            .WithColumn(nameof(AuthorModel.Name)).AsString().NotNullable().Unique();

        Create.Table(DbConstants.Publishers)
            .WithColumn(nameof(PublisherModel.Id)).AsInt32().PrimaryKey().Identity()
            .WithColumn(nameof(PublisherModel.Name)).AsString().NotNullable().Unique();

        Create.Table(DbConstants.Restrictions)
            .WithColumn(nameof(RestrictionModel.Id)).AsInt32().PrimaryKey().Identity()
            .WithColumn(nameof(RestrictionModel.Name)).AsString().NotNullable()
            .WithColumn(nameof(RestrictionModel.Value)).AsString().NotNullable();

        Create.Table(DbConstants.Languages)
            .WithColumn(nameof(LanguageModel.Id)).AsInt32().PrimaryKey().Identity()
            .WithColumn(nameof(LanguageModel.Code)).AsString().NotNullable().Unique();

        Create.Table(DbConstants.Packages)
            .WithColumn(nameof(PackageModel.Id)).AsGuid().PrimaryKey()
            .WithColumn(nameof(PackageModel.Name)).AsString().NotNullable()
            .WithColumn(nameof(PackageModel.Difficulty)).AsInt16().NotNullable()
            .WithColumn(nameof(PackageModel.PublisherId)).AsInt32().Nullable().ForeignKey(nameof(DbConstants.Publishers), nameof(PublisherModel.Id))
            .WithColumn(nameof(PackageModel.CreateDate)).AsDate().Nullable()
            .WithColumn(nameof(PackageModel.LanguageId)).AsInt32().NotNullable().ForeignKey(nameof(DbConstants.Languages), nameof(LanguageModel.Id))
            .WithColumn(nameof(PackageModel.FileName)).AsString().NotNullable()
            .WithColumn(nameof(PackageModel.LogoUri)).AsString().Nullable()
            .WithColumn(nameof(PackageModel.Downloadable)).AsBoolean().NotNullable()
            .WithColumn(nameof(PackageModel.DownloadCount)).AsInt32().NotNullable()
            .WithColumn(nameof(PackageModel.Size)).AsInt64().NotNullable()
            .WithColumn(nameof(PackageModel.Rounds)).AsJson().NotNullable()
            .WithColumn(nameof(PackageModel.QuestionCount)).AsInt16().NotNullable()
            .WithColumn(nameof(PackageModel.AtomTypesStatistic)).AsJson().NotNullable();

        Create.Table(DbConstants.PackageTags)
            .WithColumn(nameof(PackageTag.PackageId)).AsGuid().NotNullable().ForeignKey(nameof(DbConstants.Packages), nameof(PackageModel.Id))
            .WithColumn(nameof(PackageTag.TagId)).AsInt32().NotNullable().ForeignKey(nameof(DbConstants.Tags), nameof(TagModel.Id));

        Create.Table(DbConstants.PackageAuthors)
            .WithColumn(nameof(PackageAuthor.PackageId)).AsGuid().NotNullable().ForeignKey(nameof(DbConstants.Packages), nameof(PackageModel.Id))
            .WithColumn(nameof(PackageAuthor.AuthorId)).AsInt32().NotNullable().ForeignKey(nameof(DbConstants.Authors), nameof(AuthorModel.Id));

        Create.Table(DbConstants.PackageRestrictions)
            .WithColumn(nameof(PackageRestriction.PackageId)).AsGuid().NotNullable()
                .ForeignKey(nameof(DbConstants.Packages), nameof(PackageModel.Id))
            .WithColumn(nameof(PackageRestriction.RestrictionId)).AsInt32().NotNullable()
                .ForeignKey(nameof(DbConstants.Restrictions), nameof(RestrictionModel.Id));
    }

    public override void Down()
    {
        Delete.Table(DbConstants.PackageRestrictions);
        Delete.Table(DbConstants.PackageAuthors);
        Delete.Table(DbConstants.PackageTags);
        Delete.Table(DbConstants.Packages);
        Delete.Table(DbConstants.Languages);
        Delete.Table(DbConstants.Restrictions);
        Delete.Table(DbConstants.Publishers);
        Delete.Table(DbConstants.Authors);
        Delete.Table(DbConstants.Tags);
    }
}
