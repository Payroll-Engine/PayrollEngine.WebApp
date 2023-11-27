﻿using System.Collections.Generic;
using System.Net.Http;
using PayrollEngine.Client;
using PayrollEngine.Client.Model;
using PayrollEngine.Client.Service;
using PayrollEngine.Client.Service.Api;
using PayrollEngine.WebApp.Shared;

namespace PayrollEngine.WebApp.Presentation.BackendService;

public class NationalCaseDocumentBackendService(UserSession userSession, HttpClientHandler httpClientHandler,
        PayrollHttpConfiguration configuration, Localizer localizer)
    : BackendServiceBase<NationalCaseDocumentService, CaseValueServiceContext, CaseDocument, Query>(userSession, httpClientHandler, configuration, localizer)
{
    protected override CaseValueServiceContext CreateServiceContext(IDictionary<string, object> parameters = null)
    {
        var caseValueId = parameters?.GetValue<int?>(nameof(CaseChangeCaseValue.CaseChangeId));
        if (caseValueId == null)
        {
            throw new PayrollException("Missing case change for case document service");
        }
        return new(UserSession.Tenant.Id, caseValueId.Value);
    }

    protected override NationalCaseDocumentService CreateService() => 
        new(HttpClient);
}