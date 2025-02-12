using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using PayrollEngine.Client;

namespace PayrollEngine.WebApp.Server.Components.Shared;

internal static class PayrollHttpClientFactory
{
    internal static async Task<PayrollHttpClient> CreatePayrollHttpClientAsync(IConfiguration configuration)
    {
        if (configuration == null)
        {
            throw new ArgumentNullException(nameof(configuration));
        }

        // http client handler
        var clientHandler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (_, _, _, _) => true
        };

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

    private static string GetApiKey(PayrollHttpConfiguration httpConfiguration)
    {
        // priority 1: environment variable
        var apiKey = Environment.GetEnvironmentVariable(PayrollEngine.SystemSpecification.PayrollApiKey);

        // priority 2: http configuration
        apiKey ??= httpConfiguration.ApiKey;

        return apiKey;
    }
}