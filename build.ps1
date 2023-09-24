param (
    [string]$tag = "latest"
)

docker build . -f src\SIStorage.Service\Dockerfile -t vladimirkhil/sistorageservice:$tag