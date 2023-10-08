param (
    [string]$version = "1.0.0",
    [string]$apikey = ""
)

dotnet pack src\SIStorage.Service.Contract\SIStorage.Service.Contract.csproj -c Release /property:Version=$version
dotnet pack src\SIStorage.Service.Client\SIStorage.Service.Client.csproj -c Release /property:Version=$version
dotnet nuget push bin\.Release\SIStorage.Service.Contract\VKhil.SIStorage.Contract.$version.nupkg --api-key $apikey --source https://api.nuget.org/v3/index.json
dotnet nuget push bin\.Release\SIStorage.Service.Client\VKhil.SIStorage.Client.$version.nupkg --api-key $apikey --source https://api.nuget.org/v3/index.json