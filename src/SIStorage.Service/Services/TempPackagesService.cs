using Microsoft.Extensions.Options;
using SIStorage.Service.Configuration;
using SIStorage.Service.Contracts;
using SIStorage.Service.Helpers;

namespace SIStorage.Service.Services;

public sealed class TempPackagesService : ITempPackagesService
{
    private const string TempFolder = "temp";

    private readonly SIStorageOptions _options;

    public TempPackagesService(IOptions<SIStorageOptions> options)
    {
        _options = options.Value;
    }

    public string GenerateFilePath(Guid packageId)
    {
        var packagesFolder = Path.Combine(StringHelper.BuildRootedPath(_options.ContentFolder), TempFolder);
        Directory.CreateDirectory(packagesFolder);

        var fileName = Path.ChangeExtension(packageId.ToString(), "siq");
        return Path.Combine(packagesFolder, fileName);
    }

    public void Clean()
    {
        var packagesFolder = Path.Combine(StringHelper.BuildRootedPath(_options.ContentFolder), TempFolder);
        var directoryInfo = new DirectoryInfo(packagesFolder);

        if (!directoryInfo.Exists)
        {
            return;
        }

        var now = DateTime.UtcNow;

        foreach (var file in directoryInfo.EnumerateFiles())
        {
            if (now.Subtract(file.CreationTimeUtc) > _options.TempPackageLifetime)
            {
                file.Delete();
            }
        }
    }
}
