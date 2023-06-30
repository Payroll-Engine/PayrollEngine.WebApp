using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using PayrollEngine.Client.Service;
using PayrollEngine.Client.Service.Api;
using PayrollEngine.WebApp.Shared;

namespace PayrollEngine.WebApp.Presentation.BackendService;

public class TaskBackendService : BackendServiceBase<TaskService, TenantServiceContext, ViewModel.Task, Query>
{
    public TaskBackendService(UserSession userSession, IConfiguration configuration, Localizer localizer) :
        base(userSession, configuration, localizer)
    {
    }

    /// <summary>The current request context</summary>
    protected override TenantServiceContext CreateServiceContext(IDictionary<string, object> parameters = null) =>
        UserSession.Tenant != null ? new TenantServiceContext(UserSession.Tenant.Id) : null;

    /// <summary>Create the backend service</summary>
    protected override TaskService CreateService() =>
        new(HttpClient);
}