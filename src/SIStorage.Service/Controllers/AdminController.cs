using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using SIPackages;
using SIStorage.Service.Attributes;
using SIStorage.Service.Configuration;
using SIStorage.Service.Contract.Models;
using SIStorage.Service.Contract.Requests;
using SIStorage.Service.Contract.Responses;
using SIStorage.Service.Contracts;
using SIStorage.Service.Exceptions;
using SIStorage.Service.Helpers;
using System.Net;

namespace SIStorage.Service.Controllers;

/// <summary>
/// Provides admin-level API for uploading packages.
/// </summary>
[Route("api/v1/admin")]
[ApiController]
[Produces("application/json")]
public sealed class AdminController : ControllerBase
{
    private static readonly FormOptions DefaultFormOptions = new();
    private const long MaxPackageSizeBytes = 105_000_000;
    private const string PackagesFolder = "packages";
    private const string LogoFolder = "logo";

    private readonly IPackageIndexer _packageIndexer;
    private readonly IExtendedPackagesApi _packagesApi;
    private readonly SIStorageOptions _options;
    private readonly ILogger<AdminController> _logger;

    /// <summary>
    /// Initializes a new instance of <see cref="PackagesController" />.
    /// </summary>
    /// <param name="packageIndexer">Package indexer.</param>
    /// <param name="packagesApi">Packages API.</param>
    /// <param name="options">Service options.</param>
    /// <param name="logger">Service logger.</param>
    public AdminController(
        IPackageIndexer packageIndexer,
        IExtendedPackagesApi packagesApi,
        IOptions<SIStorageOptions> options,
        ILogger<AdminController> logger)
    {
        _packageIndexer = packageIndexer;
        _packagesApi = packagesApi;
        _options = options.Value;
        _logger = logger;
    }

    /// <summary>
    /// Uploads package to storage.
    /// </summary>
    [HttpPost("packages")]
    [DisableFormValueModelBinding]
    [RequestSizeLimit(MaxPackageSizeBytes)]
    public async Task<IActionResult> UploadPackage()
    {
        var cancellationToken = HttpContext.RequestAborted; // Binding is disabled so don't inject cancellation token as method parameter

        try
        {
            if (!MultipartRequestHelper.IsMultipartContentType(Request.ContentType))
            {
                throw new ServiceException(WellKnownSIStorageServiceErrorCode.MultipartContentTypeRequired, HttpStatusCode.BadRequest);
            }

            if (!Request.ContentLength.HasValue)
            {
                throw new ServiceException(WellKnownSIStorageServiceErrorCode.ContentLengthHeaderRequired, HttpStatusCode.BadRequest);
            }

            var boundary = MultipartRequestHelper.GetBoundary(
                MediaTypeHeaderValue.Parse(Request.ContentType),
                DefaultFormOptions.MultipartBoundaryLengthLimit);

            var reader = new MultipartReader(boundary, HttpContext.Request.Body);

            var section = await reader.ReadNextSectionAsync(cancellationToken);

            Guid? packageId = null;

            while (section != null)
            {
                var hasContentDispositionHeader = ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out var contentDisposition);

                if (hasContentDispositionHeader)
                {
                    if (MultipartRequestHelper.HasFileContentDisposition(contentDisposition!))
                    {
                        packageId = await ReadUploadedFileAsync(section.Body, contentDisposition!, cancellationToken);
                    }
                    else
                    {
                        throw new ServiceException(WellKnownSIStorageServiceErrorCode.ContentDispositionHeaderRequired, HttpStatusCode.BadRequest);
                    }
                }

                // Drains any remaining section body that has not been consumed and
                // reads the headers for the next section
                section = await reader.ReadNextSectionAsync(cancellationToken);
            }

            if (!packageId.HasValue)
            {
                throw new ServiceException(WellKnownSIStorageServiceErrorCode.FileEmpty, HttpStatusCode.BadRequest);
            }

            return Ok(new CreatePackageResponse(packageId.Value));
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            // No action
            return Accepted();
        }
    }

    private async Task<Guid> ReadUploadedFileAsync(
        Stream fileStream,
        ContentDispositionHeaderValue contentDisposition,
        CancellationToken cancellationToken)
    {
        var targetFilePath = Path.GetTempFileName();

        try
        {
            using (var targetStream = System.IO.File.Create(targetFilePath))
            {
                await fileStream.CopyToAsync(targetStream, cancellationToken);
            }

            var fileLength = new FileInfo(targetFilePath).Length;

            if (fileLength == 0)
            {
                throw new ServiceException(WellKnownSIStorageServiceErrorCode.FileEmpty, HttpStatusCode.BadRequest);
            }

            var maxPackageSize = 100 * 1024 * 1024;

            if (fileLength > maxPackageSize)
            {
                throw new ServiceException(
                    WellKnownSIStorageServiceErrorCode.FileTooLarge,
                    HttpStatusCode.RequestEntityTooLarge,
                    new Dictionary<string, object>
                    {
                        ["maxSizeMb"] = maxPackageSize
                    });
            }

            var packageNameValue = contentDisposition.FileName.Value
                ?? throw new ServiceException(WellKnownSIStorageServiceErrorCode.DispositionFileNameRequired, HttpStatusCode.BadRequest);

            var packageName = StringHelper.UnquoteValue(packageNameValue);

            Guid packageId;
            string packageFileName;

            using var documentStream = System.IO.File.OpenRead(targetFilePath);
            using var document = SIDocument.Load(documentStream);
            {
                if (!Guid.TryParse(document.Package.ID, out packageId))
                {
                    packageId = Guid.NewGuid();
                }

                packageFileName = Path.ChangeExtension(packageId.ToString(), "siq");
                var packageMetadata = _packageIndexer.IndexPackage(document);
                var packageLogo = document.Package.LogoItem;
                var logoUri = await GetLogoUriAsync(document, packageLogo, packageId, cancellationToken);

                await _packagesApi.AddPackageAsync(packageId, packageName, packageMetadata, packageFileName, fileLength, logoUri, cancellationToken);
            }

            var packagesFolder = Path.Combine(StringHelper.BuildRootedPath(_options.ContentFolder), PackagesFolder);
            Directory.CreateDirectory(packagesFolder);
            var packageFile = Path.Combine(packagesFolder, packageFileName);

            System.IO.File.Move(targetFilePath, packageFile);

            return packageId;
        }
        finally
        {
            if (System.IO.File.Exists(targetFilePath))
            {
                try
                {
                    System.IO.File.Delete(targetFilePath);
                }
                catch (Exception exc)
                {
                    _logger.LogWarning(exc, "File delete error: {error}", exc.Message);
                }
            }
        }
    }

    private async Task<string?> GetLogoUriAsync(SIDocument document, ContentItem? packageLogo, Guid packageId, CancellationToken cancellationToken)
    {
        if (packageLogo == null)
        {
            return null;
        }

        var media = document.TryGetMedia(packageLogo);

        if (!media.HasValue)
        {
            return null;
        }

        if (media.Value.HasStream)
        {
            var stream = media.Value.Stream;

            if (stream != null)
            {
                using (stream)
                {
                    var logoExtension = Path.GetExtension(packageLogo.Value);
                    var logoUri = Path.ChangeExtension(packageId.ToString(), logoExtension);
                    var logoFolder = Path.Combine(StringHelper.BuildRootedPath(_options.ContentFolder), LogoFolder);
                    Directory.CreateDirectory(logoFolder);
                    var logoFile = Path.Combine(logoFolder, logoUri);

                    using var logoStream = System.IO.File.Create(logoFile);
                    await stream.CopyToAsync(logoStream, cancellationToken);

                    return logoUri;
                }
            }
        }
        else if (media.Value.Uri != null)
        {
            return media.Value.Uri.AbsolutePath;
        }

        return null;
    }

    /// <summary>
    /// Creates temporary random package.
    /// </summary>
    /// <param name="packageParameters">Random package parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Created package info.</returns>
    /// <remarks>This method is not affected by rate limiter.</remarks>
    [HttpPost("random")]
    public Task<Contract.Models.Package> PostRandomAsync(
        RandomPackageParameters packageParameters,
        CancellationToken cancellationToken = default) => _packagesApi.GetRandomPackageAsync(packageParameters, cancellationToken);
}
