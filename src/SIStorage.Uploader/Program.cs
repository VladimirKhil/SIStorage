using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SIPackages;
using SIStorage.Service.Client;
using SIStorage.Service.Contract;

var packageFolder = args[0];

var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

var services = new ServiceCollection();
services.AddSIStorageServiceClient(configuration);

var sp = services.BuildServiceProvider();
var client = sp.GetRequiredService<ISIStorageServiceClient>();

var packageCounter = 1;

await ExportFolderAsync(new DirectoryInfo(packageFolder));

async Task ExportFolderAsync(DirectoryInfo directoryInfo)
{
    foreach (var directory in directoryInfo.EnumerateDirectories())
    {
        await ExportFolderAsync(directory);
    }

    foreach (var file in directoryInfo.EnumerateFiles())
    {
        await UploadFileAsync(file.FullName);
    }
}

async Task UploadFileAsync(string fullName)
{
    using (var docStream = File.OpenRead(fullName))
    using (var doc = SIDocument.Load(docStream))
    {
        var packageIdString = doc.Package.ID;

        if (Guid.TryParse(packageIdString, out var packageId))
        {
            try
            {
                var p = await client.Packages.GetPackageAsync(packageId);

                if (p != null)
                {
                    Console.WriteLine($"{packageCounter++}: already loaded {fullName}");
                    return;
                }
            }
            catch (Exception)
            {
                // Package already exists
            }
        }
    }

    using var stream = File.OpenRead(fullName);
    await client.Admin.UploadPackageAsync(Path.GetFileNameWithoutExtension(fullName), stream);

    Console.WriteLine($"{packageCounter++}: {fullName}");
}