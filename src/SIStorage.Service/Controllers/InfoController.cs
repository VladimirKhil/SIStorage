using Microsoft.AspNetCore.Mvc;
using SIStorage.Service.Contract.Responses;

namespace SIStorage.Service.Controllers;

[Route("api/v1/info")]
[ApiController]
public sealed class InfoController : ControllerBase
{
    private static readonly StorageInfo _storageInfo = new() { RandomPackagesSupported = true };

    [HttpGet]
    public Task<StorageInfo> GetAsync() => Task.FromResult(_storageInfo);
}
