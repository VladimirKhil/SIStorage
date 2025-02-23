using SIStorage.Service.Client.Properties;
using SIStorage.Service.Contract;
using SIStorage.Service.Contract.Models;
using SIStorage.Service.Contract.Requests;
using SIStorage.Service.Contract.Responses;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace SIStorage.Service.Client;

/// <inheritdoc />
internal sealed class AdminApi : IAdminApi
{
    private const int BufferSize = 80 * 1024;
    private const string RequestBodyTooLargeError = "Request body too large.";

    private readonly HttpClient _client;

    /// <summary>
    /// Initializes a new instance of <see cref="AdminApi" /> class.
    /// </summary>
    /// <param name="client">HTTP client to use.</param>
    public AdminApi(HttpClient client) => _client = client;

    public async Task<CreatePackageResponse> UploadPackageAsync(string packageName, Stream packageStream, CancellationToken cancellationToken = default)
    {
        using var content = new StreamContent(packageStream, BufferSize);

        try
        {
            using var formData = new MultipartFormDataContent
            {
                { content, "file", packageName }
            };

            using var response = await _client.PostAsync("admin/packages", formData, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await GetErrorMessageAsync(response, cancellationToken);
                throw new Exception(errorMessage);
            }

            return await response.Content.ReadFromJsonAsync<CreatePackageResponse>(cancellationToken: cancellationToken)
                ?? throw new HttpRequestException("", null, statusCode: HttpStatusCode.NoContent);
        }
        catch (HttpRequestException exc)
        {
            throw new Exception(Resources.UploadFileConnectionError, exc.InnerException ?? exc);
        }
        catch (TaskCanceledException exc)
        {
            if (!exc.CancellationToken.IsCancellationRequested)
            {
                throw new Exception(Resources.UploadFileTimeout, exc);
            }

            throw;
        }
    }

    public async Task ReindexAsync(CancellationToken cancellationToken = default)
    {
        using var response = await _client.PostAsync("admin/reindex", null, cancellationToken);
        
        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException(await response.Content.ReadAsStringAsync(cancellationToken), null, response.StatusCode);
        }
    }

    public async Task DeletePackageAsync(Guid packageId, CancellationToken cancellationToken = default)
    {
        using var response = await _client.DeleteAsync($"admin/packages/{packageId}", cancellationToken);
       
        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException(await response.Content.ReadAsStringAsync(cancellationToken), null, response.StatusCode);
        }
    }

    public async Task<Package> GetRandomPackageAsync(RandomPackageParameters randomPackageParameters, CancellationToken cancellationToken = default)
    {
        using var response = await _client.PostAsJsonAsync("packages/random", randomPackageParameters, cancellationToken: cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException(await response.Content.ReadAsStringAsync(cancellationToken), null, response.StatusCode);
        }

        var package = await response.Content.ReadFromJsonAsync<Package>(cancellationToken: cancellationToken)
            ?? throw new Exception(WellKnownSIStorageServiceErrorCode.PackageNotFound.ToString());

        return package;
    }

    private static async Task<string> GetErrorMessageAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        var serverError = await response.Content.ReadAsStringAsync(cancellationToken);

        if (response.StatusCode == HttpStatusCode.RequestEntityTooLarge
            || response.StatusCode == HttpStatusCode.BadRequest && serverError == RequestBodyTooLargeError)
        {
            return Resources.FileTooLarge;
        }

        if (response.StatusCode == HttpStatusCode.BadGateway)
        {
            return $"{response.StatusCode}: Bad Gateway";
        }

        if (response.StatusCode == HttpStatusCode.TooManyRequests)
        {
            return $"{response.StatusCode}: Too many requests. Try again later";
        }

        try
        {
            var error = JsonSerializer.Deserialize<SIStorageServiceError>(serverError);

            if (error != null)
            {
                return error.ErrorCode.ToString();
            }
        }
        catch // Invalid JSON or wrong type
        {

        }

        return $"{response.StatusCode}: {serverError}";
    }
}
