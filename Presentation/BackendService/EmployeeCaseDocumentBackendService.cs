using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using PayrollEngine.Client.Model;
using PayrollEngine.Client.Service;
using PayrollEngine.Client.Service.Api;
using PayrollEngine.WebApp.Shared;

namespace PayrollEngine.WebApp.Presentation.BackendService;

public class EmployeeCaseDocumentBackendService : BackendServiceBase<EmployeeCaseDocumentService, EmployeeCaseValueServiceContext, CaseDocument, Query>
{
    public EmployeeCaseDocumentBackendService(UserSession userSession, IConfiguration configuration, Localizer localizer) :
        base(userSession, configuration, localizer)
    {
    }

    protected override EmployeeCaseValueServiceContext CreateServiceContext(IDictionary<string, object> parameters = null)
    {
        var caseValueId = parameters?.GetValue<int>(nameof(CaseChangeCaseValue.CaseChangeId));
        if (caseValueId == null)
        {
            throw new PayrollException("Missing case change for case document service");
        }
        return new(UserSession.Tenant.Id, UserSession.Employee.Id, caseValueId.Value);
    }

    protected override EmployeeCaseDocumentService CreateService() =>
        new(HttpClient);
}