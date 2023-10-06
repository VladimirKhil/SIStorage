using SIStorage.Service.Contract.Models;
using SIStorage.Service.Exceptions;

namespace SIStorage.Service.Middlewares;

/// <summary>
/// Handles exceptions and creates corresponsing service responses.
/// </summary>
internal sealed class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ErrorHandlingMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ServiceException exc)
        {
            context.Response.StatusCode = (int)exc.StatusCode;
            await context.Response.WriteAsJsonAsync(new SIStorageServiceError { ErrorCode = exc.ErrorCode });
        }
    }
}
