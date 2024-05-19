using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using Polly;
using Polly.Extensions.Http;
using SIStorage.Service.Contract;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace SIStorage.Service.Client;

/// <summary>
/// Provides an extension method for adding <see cref="ISIStorageServiceClient" /> implementation to service collection.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds <see cref="ISIStorageServiceClient" /> implementation to service collection.
    /// </summary>
    /// <param name="services">Service collection.</param>
    /// <param name="configuration">App configuration.</param>
    public static IServiceCollection AddSIStorageServiceClient(this IServiceCollection services, IConfiguration configuration)
    {
        var optionsSection = configuration.GetSection(SIStorageClientOptions.ConfigurationSectionName);
        services.Configure<SIStorageClientOptions>(optionsSection);

        var options = optionsSection.Get<SIStorageClientOptions>();
        
        services.AddHttpClient<ISIStorageServiceClient, SIStorageServiceClient>(
            client =>
            {
                if (options != null)
                {
                    var serviceUri = options.ServiceUri;
                    client.BaseAddress = serviceUri != null ? new Uri(serviceUri, "api/v1/") : null;
                    client.Timeout = options.Timeout;

                    SetAuthSecret(options, client);
                }

                client.DefaultRequestVersion = HttpVersion.Version20;
            })
            .AddPolicyHandler(HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(
                    options?.RetryCount ?? SIStorageClientOptions.DefaultRetryCount,
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(1.5, retryAttempt))));

        services.AddSingleton<ISIStorageClientFactory, SIStorageClientFactory>();

        return services;
    }

    /// <summary>
    /// Allows to create custom SIStorageService client.
    /// </summary>
    /// <remarks>
    /// This method is intended to be used in desktop client app code only.
    /// </remarks>
    /// <param name="options">Client options.</param>
    public static ISIStorageServiceClient CreateSIStorageServiceClient(SIStorageClientOptions options)
    {
        var cookieContainer = new CookieContainer();
        HttpMessageHandler handler = new HttpClientHandler { CookieContainer = cookieContainer };

        var policyHandler = new PolicyHttpMessageHandler(
            HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(
                    options.RetryCount,
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(1.5, retryAttempt))))
        {
            InnerHandler = handler
        };

        var client = new HttpClient(policyHandler)
        {
            BaseAddress = options.ServiceUri,
            Timeout = options.Timeout,
            DefaultRequestVersion = HttpVersion.Version20
        };

        SetAuthSecret(options, client);

        return new SIStorageServiceClient(client);
    }

    private static void SetAuthSecret(SIStorageClientOptions options, HttpClient client)
    {
        if (options.ClientSecret == null)
        {
            return;
        }

        var authHeader = Convert.ToBase64String(Encoding.ASCII.GetBytes($"admin:{options.ClientSecret}"));
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authHeader);
    }
}
