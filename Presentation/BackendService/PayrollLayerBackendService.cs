using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using PayrollEngine.Client.Service;
using PayrollEngine.Client.Service.Api;
using PayrollEngine.WebApp.Shared;
using PayrollEngine.WebApp.ViewModel;

namespace PayrollEngine.WebApp.Presentation.BackendService;

public class PayrollLayerBackendService : BackendServiceBase<PayrollLayerService, PayrollServiceContext, PayrollLayer, Query>
{
    public PayrollLayerBackendService(UserSession userSession, IConfiguration configuration,
        Localizer localizer) :
        base(userSession, configuration, localizer)
    {
    }

    /// <summary>The current request context</summary>
    protected override PayrollServiceContext CreateServiceContext(IDictionary<string, object> parameters = null)
    {
        if (UserSession.Tenant != null && UserSession.Payroll != null)
        {
            return new(UserSession.Tenant.Id, UserSession.Payroll.Id);
        }
        return null;
    }

    /// <summary>Create the backend service</summary>
    protected override PayrollLayerService CreateService() =>
        new(HttpClient);
}
