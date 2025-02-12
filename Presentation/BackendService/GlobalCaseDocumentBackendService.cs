using System.Collections.Generic;
using PayrollEngine.Client;
using PayrollEngine.Client.Model;
using PayrollEngine.Client.Service;
using PayrollEngine.Client.Service.Api;
using PayrollEngine.WebApp.Shared;

namespace PayrollEngine.WebApp.Presentation.BackendService;

public class GlobalCaseDocumentBackendService(UserSession userSession, 
    PayrollHttpClient httpClient,
    Localizer localizer)
    : BackendServiceBase<GlobalCaseDocumentService, CaseValueServiceContext, CaseDocument, Query>(
        userSession, httpClient, localizer)
{
    protected override CaseValueServiceContext CreateServiceContext(IDictionary<string, object> parameters = null)
    {
        var caseValueId = parameters?.GetValue<int?>(nameof(CaseChangeCaseValue.CaseChangeId));
        if (caseValueId == null)
        {
            throw new PayrollException("Missing case change for case document service.");
        }
        return new(UserSession.Tenant.Id, caseValueId.Value);
    }

    protected override GlobalCaseDocumentService CreateService() => 
        new(HttpClient);
}