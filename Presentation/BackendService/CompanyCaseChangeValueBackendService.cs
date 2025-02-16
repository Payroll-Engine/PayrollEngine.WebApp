﻿using System.Collections.Generic;
using PayrollEngine.Client;
using PayrollEngine.Client.Model;
using PayrollEngine.WebApp.Shared;

namespace PayrollEngine.WebApp.Presentation.BackendService;

public class CompanyCaseChangeValueBackendService(UserSession userSession, 
    PayrollHttpClient httpClient,
    ILocalizerService localizerService)
    : CaseChangeValueBackendServiceBase(userSession, httpClient, localizerService)
{
    protected override void SetupReadQuery(PayrollCaseChangeQuery query, IDictionary<string, object> parameters = null)
    {
        base.SetupReadQuery(query, parameters);
        query.CaseType = CaseType.Company;
        // division filter
        if (UserSession.Payroll != null)
        {
            query.DivisionId = UserSession.Payroll.DivisionId;
        }
    }
}