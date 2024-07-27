using Microsoft.AspNetCore.Mvc;
using SIStorage.Service.Contract;
using SIStorage.Service.Contract.Models;

namespace SIStorage.Service.Controllers;

/// <summary>
/// Provide API for querying packages facets.
/// </summary>
/// <remarks>
/// Initializes a new instance of <see cref="FacetsController" />.
/// </remarks>
/// <param name="facetsApi">Facets API.</param>
[Route("api/v1/facets")]
[ApiController]
[Produces("application/json")]
public sealed class FacetsController(IFacetsApi facetsApi) : ControllerBase
{
    /// <summary>
    /// Gets packages publishers.
    /// </summary>
    /// <param name="languageId">Language identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Packages publishers</returns>
    [HttpGet("publishers")]
    public Task<Publisher[]> GetPublishersAsync(int? languageId = null, CancellationToken cancellationToken = default) =>
        facetsApi.GetPublishersAsync(languageId, cancellationToken);

    /// <summary>
    /// Gets packages authors.
    /// </summary>
    /// <param name="languageId">Language identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Packages authors</returns>
    [HttpGet("authors")]
    [ProducesResponseType(typeof(Author[]), StatusCodes.Status200OK)]
    public Task<Author[]> GetAuthorsAsync(int? languageId = null, CancellationToken cancellationToken = default) =>
        facetsApi.GetAuthorsAsync(languageId, cancellationToken);
    
    /// <summary>
    /// Gets packages tags.
    /// </summary>
    /// <param name="languageId">Language identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Packages tags</returns>
    [HttpGet("tags")]
    public Task<Tag[]> GetTagsAsync(int? languageId = null, CancellationToken cancellationToken = default) =>
        facetsApi.GetTagsAsync(languageId, cancellationToken);

    /// <summary>
    /// Gets packages restrictions.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Packages restrictions</returns>
    [HttpGet("restrictions")]
    public Task<Restriction[]> GetRestrictionsAsync(CancellationToken cancellationToken = default) =>
        facetsApi.GetRestrictionsAsync(cancellationToken);

    /// <summary>
    /// Gets supported languages.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Supported languages.</returns>
    [HttpGet("languages")]
    public Task<Language[]> GetLanguagesAsync(CancellationToken cancellationToken = default) =>
        facetsApi.GetLanguagesAsync(cancellationToken);
}
