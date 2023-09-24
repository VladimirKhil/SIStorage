using SIStorage.Service.Contract.Models;

namespace SIStorage.Service.Contract;

/// <summary>
/// Provides API for working with facets.
/// </summary>
public interface IFacetsApi
{
    /// <summary>
    /// Gets well-known package publishers.
    /// </summary>
    /// <param name="languageId">Language identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<Publisher[]> GetPublishersAsync(int? languageId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets well-known package authors.
    /// </summary>
    /// <param name="languageId">Language identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<Author[]> GetAuthorsAsync(int? languageId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets well-known package tags.
    /// </summary>
    /// <param name="languageId">Language identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<Tag[]> GetTagsAsync(int? languageId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets well-known package restrictions.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<Restriction[]> GetRestrictionsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets well-known languages.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<Language[]> GetLanguagesAsync(CancellationToken cancellationToken = default);
}
