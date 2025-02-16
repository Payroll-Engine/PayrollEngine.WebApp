using System.Collections.Generic;
using PayrollEngine.Client;
using PayrollEngine.Client.Service;
using PayrollEngine.Client.Service.Api;
using PayrollEngine.WebApp.Shared;

namespace PayrollEngine.WebApp.Presentation.BackendService;

public class DivisionBackendService(UserSession userSession,
    PayrollHttpClient httpClient,
    ILocalizerService localizerService)
    : BackendServiceBase<DivisionService, TenantServiceContext, ViewModel.Division, Query>(
        userSession, httpClient, localizerService)
{
    /// <summary>The current request context</summary>
    protected override TenantServiceContext CreateServiceContext(IDictionary<string, object> parameters = null) =>
        UserSession.Tenant != null ? new TenantServiceContext(UserSession.Tenant.Id) : null;

    /// <summary>Create the backend service</summary>
    protected override DivisionService CreateService() =>
        new(HttpClient);
}