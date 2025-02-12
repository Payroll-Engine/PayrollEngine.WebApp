using System.Collections.Generic;
using PayrollEngine.Client;
using PayrollEngine.Client.Service;
using PayrollEngine.Client.Service.Api;
using PayrollEngine.WebApp.Shared;

namespace PayrollEngine.WebApp.Presentation.BackendService;

public class LogBackendService(UserSession userSession, 
    PayrollHttpClient httpClient,
    Localizer localizer)
    : BackendServiceBase<LogService, TenantServiceContext, ViewModel.Log, Query>(
        userSession, httpClient, localizer)
{
    /// <summary>The current request context</summary>
    protected override TenantServiceContext CreateServiceContext(IDictionary<string, object> parameters = null) =>
        UserSession.Tenant != null ? new TenantServiceContext(UserSession.Tenant.Id) : null;

    /// <summary>Create the backend service</summary>
    protected override LogService CreateService() =>
        new(HttpClient);
}