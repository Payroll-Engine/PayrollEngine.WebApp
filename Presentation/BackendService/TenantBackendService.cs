using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using PayrollEngine.Client.Service;
using PayrollEngine.Client.Service.Api;
using PayrollEngine.WebApp.Shared;

namespace PayrollEngine.WebApp.Presentation.BackendService;

public class TenantBackendService : BackendServiceBase<TenantService, RootServiceContext, ViewModel.Tenant, Query>
{
    public TenantBackendService(UserSession userSession, IConfiguration configuration, Localizer localizer) :
        base(userSession, configuration, localizer)
    {
    }

    /// <summary>The current request context</summary>
    protected override RootServiceContext CreateServiceContext(IDictionary<string, object> parameters = null) => new();

    /// <summary>Create the backend service</summary>
    protected override TenantService CreateService() =>
        new(HttpClient);
}