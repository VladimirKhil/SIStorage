using Microsoft.AspNetCore.Mvc;
using SIStorage.Service.Contract;
using SIStorage.Service.Contract.Models;

namespace SIStorage.Service.Controllers;

/// <summary>
/// Provide API for querying packages facets.
/// </summary>
[Route("api/v1/facets")]
[ApiController]
[Produces("application/json")]
public sealed class FacetsController : ControllerBase
{
    private readonly IFacetsApi _facetsApi;

    /// <summary>
    /// Initializes a new instance of <see cref="FacetsController" />.
    /// </summary>
    /// <param name="facetsApi">Facets API.</param>
    public FacetsController(IFacetsApi facetsApi) => _facetsApi = facetsApi;

    /// <summary>
    /// Gets packages publishers.
    /// </summary>
    /// <param name="languageId">Language identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Packages publishers</returns>
    [HttpGet("publishers")]
    public Task<Publisher[]> GetPublishersAsync(int? languageId = null, CancellationToken cancellationToken = default) =>
        _facetsApi.GetPublishersAsync(languageId, cancellationToken);

    /// <summary>
    /// Gets packages authors.
    /// </summary>
    /// <param name="languageId">Language identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Packages authors</returns>
    [HttpGet("authors")]
    [ProducesResponseType(typeof(Author[]), StatusCodes.Status200OK)]
    public Task<Author[]> GetAuthorsAsync(int? languageId = null, CancellationToken cancellationToken = default) =>
        _facetsApi.GetAuthorsAsync(languageId, cancellationToken);
    
    /// <summary>
    /// Gets packages tags.
    /// </summary>
    /// <param name="languageId">Language identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Packages tags</returns>
    [HttpGet("tags")]
    public Task<Tag[]> GetTagsAsync(int? languageId = null, CancellationToken cancellationToken = default) =>
        _facetsApi.GetTagsAsync(languageId, cancellationToken);

    /// <summary>
    /// Gets packages restrictions.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Packages restrictions</returns>
    [HttpGet("restrictions")]
    public Task<Restriction[]> GetRestrictionsAsync(CancellationToken cancellationToken = default) =>
        _facetsApi.GetRestrictionsAsync(cancellationToken);

    /// <summary>
    /// Gets supported languages.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Supported languages.</returns>
    [HttpGet("languages")]
    public Task<Language[]> GetLanguagesAsync(CancellationToken cancellationToken = default) =>
        _facetsApi.GetLanguagesAsync(cancellationToken);
}
