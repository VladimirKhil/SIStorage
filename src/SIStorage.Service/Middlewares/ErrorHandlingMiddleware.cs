using SIStorage.Service.Contract.Models;
using SIStorage.Service.Exceptions;

namespace SIStorage.Service.Middlewares;

/// <summary>
/// Handles exceptions and creates corresponsing service responses.
/// </summary>
internal sealed class ErrorHandlingMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (ServiceException exc)
        {
            context.Response.StatusCode = (int)exc.StatusCode;
            await context.Response.WriteAsJsonAsync(new SIStorageServiceError { ErrorCode = exc.ErrorCode }, SIStorageContext.Default.SIStorageServiceError);
        }
    }
}
