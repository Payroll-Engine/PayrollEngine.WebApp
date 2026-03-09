using System;
using System.Net.Http;
using System.Security.Authentication;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using PayrollEngine.Client;

namespace PayrollEngine.WebApp.Server.Components.Shared;

/// <summary>
/// Factory for creating configured <see cref="PayrollHttpClient"/> instances
/// </summary>
internal static class PayrollHttpClientFactory
{
    /// <summary>
    /// Create a new payroll HTTP client with SSL and API key configuration
    /// </summary>
    /// <param name="configuration">The application configuration</param>
    /// <returns>A configured payroll HTTP client</returns>
    internal static async Task<PayrollHttpClient> CreatePayrollHttpClientAsync(IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        // http client handler
        var clientHandler = new HttpClientHandler
        {
            SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13
        };

        // insecure ssl: skip certificate validation (dev only)
        var allowInsecureSsl = configuration.GetValue<bool>("AllowInsecureSsl");
        if (allowInsecureSsl)
        {
            Log.Warning("SSL certificate validation is disabled (AllowInsecureSsl=true)");
            clientHandler.ServerCertificateCustomValidationCallback = (_, _, _, _) => true;
        }

        // http client configuration
        var httpConfiguration = await configuration.GetHttpConfigurationAsync();
        if (httpConfiguration == null)
        {
            throw new PayrollException("Missing Payroll HTTP configuration.");
        }
        if (!httpConfiguration.Valid())
        {
            throw new PayrollException("Invalid Payroll HTTP configuration.");
        }
        // http client
        var httpClient = new PayrollHttpClient(clientHandler, httpConfiguration);

        // api key
        var apiKey = GetApiKey(httpConfiguration);
        if (!string.IsNullOrWhiteSpace(apiKey))
        {
            httpClient.SetApiKey(apiKey);
        }
        return httpClient;
    }

    /// <summary>
    /// Get the API key from environment or configuration
    /// </summary>
    /// <param name="httpConfiguration">The HTTP configuration</param>
    /// <returns>The API key, or null if not configured</returns>
    private static string GetApiKey(PayrollHttpConfiguration httpConfiguration)
    {
        // priority 1: environment variable
        var apiKey = Environment.GetEnvironmentVariable(PayrollEngine.SystemSpecification.PayrollApiKey);

        // priority 2: http configuration
        apiKey ??= httpConfiguration.ApiKey;

        return apiKey;
    }
}
