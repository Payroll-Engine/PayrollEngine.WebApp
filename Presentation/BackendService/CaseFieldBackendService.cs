using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using PayrollEngine.Client.Service;
using PayrollEngine.Client.Service.Api;
using PayrollEngine.WebApp.ViewModel;

namespace PayrollEngine.WebApp.Presentation.BackendService;

public class CaseFieldBackendService : BackendServiceBase<CaseFieldService, CaseServiceContext, RegulationCaseField, Query>
{
    public CaseFieldBackendService(UserSession userSession, IConfiguration configuration) :
        base(userSession, configuration)
    {
    }

    /// <summary>The current request context</summary>
    protected override CaseServiceContext CreateServiceContext(IDictionary<string, object> parameters = null)
    {
        var parentCase = parameters?.GetValue<IRegulationItem>(nameof(IRegulationItem.Parent));
        if (parentCase == null)
        {
            throw new PayrollException("Missing case for case field service");
        }

        if (UserSession.Tenant != null && UserSession.Payroll != null)
        {
            return new(UserSession.Tenant.Id, UserSession.Payroll.Id, parentCase.Id);
        }
        return null;
    }

    /// <summary>Create the backend service</summary>
    protected override CaseFieldService CreateService(IDictionary<string, object> parameters = null) =>
        new(HttpClient);
}