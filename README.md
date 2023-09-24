# SIStorage

Provides a SIStorage service for SIGame packages and .NET and web clients for the service.

Allows to search packages with custom filters and sorting with paging support.

The service does not operate packages files. It only manipulates packages metadata stored in a PostgreSQL database.

Current implementation does not contain an API for filling and updating the storage; this should be done manually.

# Localization

Current architecture supposes that the SIStorage contains a predefines set of languages in Languages table.

Each package contains a languageId link to a language in that table.

Every publisher, author and tag are linked only to packages of certain language.

Restrictions are language-independent.

# Build

`dotnet build src\SIStorage.Service\SIStorage.Service.csproj`

.NET 7 SDK or higher is required to compile the source code.

Web:

`npm install
npm run build-prod`

NPM is required to compile the web client source code.

# Dependences

SIStorage is build upon Linq2DB, FluentMigrator, AutoMapper and Polly.

# Deploy

You can deploy standalone SIStorage service to Docker and test it with .NET Core client.

SIStorage service Docker: https://hub.docker.com/repository/docker/vladimirkhil/sistorageservice

SIStorage service .NET client NuGet package: https://www.nuget.org/packages/SIStorage.Service.Client

SIStorage service web client NPM package: https://www.npmjs.com/package/sistorage-client

Helm usage in Chart.yaml dependencies:

```
- name: sistorage
  version: "1.0.0"
  repository: "https://vladimirkhil.github.io/SIStorage/helm/repo"
```
