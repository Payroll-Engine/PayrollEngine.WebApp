using System.Collections.Generic;
using PayrollEngine.Client;
using PayrollEngine.Client.Service;
using PayrollEngine.Client.Service.Api;
using PayrollEngine.WebApp.Shared;

namespace PayrollEngine.WebApp.Presentation.BackendService;

public class TenantBackendService(UserSession userSession, PayrollHttpClient httpClient,
    ILocalizerService localizerService)
    : BackendServiceBase<TenantService, RootServiceContext, ViewModel.Tenant, Query>(
        userSession, httpClient, localizerService)
{
    /// <summary>The current request context</summary>
    protected override RootServiceContext CreateServiceContext(IDictionary<string, object> parameters = null) => new();

    /// <summary>Create the backend service</summary>
    protected override TenantService CreateService() =>
        new(HttpClient);
}