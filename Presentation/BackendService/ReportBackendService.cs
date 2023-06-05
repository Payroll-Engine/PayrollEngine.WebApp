using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using PayrollEngine.Client.Service;
using PayrollEngine.Client.Service.Api;

namespace PayrollEngine.WebApp.Presentation.BackendService;

public class ReportBackendService : BackendServiceBase<ReportService, RegulationServiceContext, ViewModel.Report, Query>
{
    public ReportBackendService(UserSession userSession, IConfiguration configuration) :
        base(userSession, configuration)
    {
    }

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
    protected override ReportService CreateService(IDictionary<string, object> parameters = null) =>
        new(HttpClient);
}