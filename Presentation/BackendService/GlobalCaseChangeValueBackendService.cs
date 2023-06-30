using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using PayrollEngine.Client.Model;
using PayrollEngine.WebApp.Shared;

namespace PayrollEngine.WebApp.Presentation.BackendService;

public class GlobalCaseChangeValueBackendService : CaseChangeValueBackendServiceBase
{
    public GlobalCaseChangeValueBackendService(UserSession userSession, IConfiguration configuration, Localizer localizer) :
        base(userSession, configuration, localizer)
    {
    }

    protected override void SetupReadQuery(PayrollCaseChangeQuery query, IDictionary<string, object> parameters = null)
    {
        base.SetupReadQuery(query, parameters);
        query.CaseType = CaseType.Global;
        // division
        if (UserSession.Payroll != null)
        {
            query.DivisionId = UserSession.Payroll.DivisionId;
        }
    }
}