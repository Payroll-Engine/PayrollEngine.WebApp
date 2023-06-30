using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using PayrollEngine.Client.Service;
using PayrollEngine.Client.Service.Api;
using PayrollEngine.WebApp.Shared;

namespace PayrollEngine.WebApp.Presentation.BackendService;

public class LogBackendService : BackendServiceBase<LogService, TenantServiceContext, ViewModel.Log, Query>
{
    public LogBackendService(UserSession userSession, IConfiguration configuration, Localizer localizer) :
        base(userSession, configuration, localizer)
    {
    }

    /// <summary>The current request context</summary>
    protected override TenantServiceContext CreateServiceContext(IDictionary<string, object> parameters = null) =>
        UserSession.Tenant != null ? new TenantServiceContext(UserSession.Tenant.Id) : null;

    /// <summary>Create the backend service</summary>
    protected override LogService CreateService() =>
        new(HttpClient);
}