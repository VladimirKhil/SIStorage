using Microsoft.AspNetCore.Mvc;
using SIStorage.Service.Contract.Models;
using SIStorage.Service.Contract.Responses;

namespace SIStorage.Service.Controllers;

/// <summary>
/// Provides general service information.
/// </summary>
[Route("api/v1/info")]
[ApiController]
[Produces("application/json")]
public sealed class InfoController : ControllerBase
{
    private static readonly StorageInfo _storageInfo = new(true, false);

    /// <summary>
    /// Provides general service information.
    /// </summary>
    /// <returns>General service information.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(StorageInfo), StatusCodes.Status200OK)]
    public Task<StorageInfo> GetAsync() => Task.FromResult(_storageInfo);
}
