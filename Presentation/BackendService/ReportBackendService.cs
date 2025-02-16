using System.Collections.Generic;
using PayrollEngine.Client;
using PayrollEngine.Client.Service;
using PayrollEngine.Client.Service.Api;
using PayrollEngine.WebApp.Shared;

namespace PayrollEngine.WebApp.Presentation.BackendService;

public class ReportBackendService(UserSession userSession,
    PayrollHttpClient httpClient, 
    ILocalizerService localizerService)
    : BackendServiceBase<ReportService, RegulationServiceContext, ViewModel.Report, Query>(
        userSession, httpClient, localizerService)
{
    /// <summary>The current request context</summary>
    protected override RegulationServiceContext CreateServiceContext(IDictionary<string, object> parameters = null)
    {
        if (UserSession.Tenant != null && UserSession.Payroll != null)
        {
            return new(UserSession.Tenant.Id, UserSession.Payroll.Id);
        }
        return null;
    }

    /// <summary>Create the backend service</summary>
    protected override ReportService CreateService() =>
        new(HttpClient);
}