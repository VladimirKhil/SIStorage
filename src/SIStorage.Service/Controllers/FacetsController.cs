using Microsoft.AspNetCore.Mvc;
using SIStorage.Service.Contract;
using SIStorage.Service.Contract.Models;

namespace SIStorage.Service.Controllers;

[Route("api/v1/facets")]
[ApiController]
public sealed class FacetsController : ControllerBase
{
    private readonly IFacetsApi _facetsApi;

    public FacetsController(IFacetsApi facetsApi) => _facetsApi = facetsApi;

    [HttpGet("publishers")]
    public Task<Publisher[]> GetPublishersAsync(int? languageId = null, CancellationToken cancellationToken = default) =>
        _facetsApi.GetPublishersAsync(languageId, cancellationToken);

    [HttpGet("authors")]
    public Task<Author[]> GetAuthorsAsync(int? languageId = null, CancellationToken cancellationToken = default) =>
        _facetsApi.GetAuthorsAsync(languageId, cancellationToken);

    [HttpGet("tags")]
    public Task<Tag[]> GetTagsAsync(int? languageId = null, CancellationToken cancellationToken = default) =>
        _facetsApi.GetTagsAsync(languageId, cancellationToken);

    [HttpGet("restrictions")]
    public Task<Restriction[]> GetRestrictionsAsync(CancellationToken cancellationToken = default) =>
        _facetsApi.GetRestrictionsAsync(cancellationToken);

    [HttpGet("languages")]
    public Task<Language[]> GetLanguagesAsync(CancellationToken cancellationToken = default) =>
        _facetsApi.GetLanguagesAsync(cancellationToken);
}
