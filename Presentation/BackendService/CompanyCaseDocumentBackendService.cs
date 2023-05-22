using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using PayrollEngine.Client.Model;
using PayrollEngine.Client.Service;
using PayrollEngine.Client.Service.Api;

namespace PayrollEngine.WebApp.Presentation.BackendService;

public class CompanyCaseDocumentBackendService : BackendServiceBase<CompanyCaseDocumentService, CaseValueServiceContext, CaseDocument, Query>
{
    public CompanyCaseDocumentBackendService(UserSession userSession, IConfiguration configuration) :
        base(userSession, configuration)
    {
    }

    protected override CaseValueServiceContext CreateServiceContext(IDictionary<string, object> parameters = null)
    {
        var caseValueId = parameters?.GetValue<int?>(nameof(CaseChangeCaseValue.CaseChangeId));
        if (caseValueId == null)
        {
            throw new PayrollException("Missing case change for case document service");
        }
        return new(UserSession.Tenant.Id, caseValueId.Value);
    }

    protected override CompanyCaseDocumentService CreateService(IDictionary<string, object> parameters = null) =>
        new(HttpClient);
}