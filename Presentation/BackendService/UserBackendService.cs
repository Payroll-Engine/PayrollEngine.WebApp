using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using PayrollEngine.Client.Service;
using PayrollEngine.Client.Service.Api;

namespace PayrollEngine.WebApp.Presentation.BackendService;

public class UserBackendService : BackendServiceBase<UserService, TenantServiceContext, ViewModel.User, Query>
{
    public UserBackendService(UserSession userSession, IConfiguration configuration) :
        base(userSession, configuration)
    {
    }

    /// <summary>The current request context</summary>
    protected override TenantServiceContext CreateServiceContext(IDictionary<string, object> parameters = null) =>
        UserSession.Tenant != null ? new TenantServiceContext(UserSession.Tenant.Id) : null;

    /// <summary>Create the backend service</summary>
    protected override UserService CreateService(IDictionary<string, object> parameters = null) =>
        new(HttpClient);
}