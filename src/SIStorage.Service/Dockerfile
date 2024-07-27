FROM mcr.microsoft.com/dotnet/runtime-deps:8.0-alpine AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/SIStorage.Service.Contract/SIStorage.Service.Contract.csproj", "src/SIStorage.Service.Contract/"]
COPY ["src/SIStorage.Database/SIStorage.Database.csproj", "src/SIStorage.Database/"]
COPY ["src/SIStorage.Service/SIStorage.Service.csproj", "src/SIStorage.Service/"]
RUN dotnet restore "src/SIStorage.Service/SIStorage.Service.csproj" -r linux-musl-x64
COPY . .
WORKDIR "/src/src/SIStorage.Service"
RUN dotnet publish "SIStorage.Service.csproj" -p:PublishSingleFile=true -r linux-musl-x64 -p:PublishTrimmed=true -c $BUILD_CONFIGURATION -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["./SIStorage.Service"]