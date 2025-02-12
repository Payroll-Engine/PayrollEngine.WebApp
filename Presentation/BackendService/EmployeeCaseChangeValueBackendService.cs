using System.Collections.Generic;
using PayrollEngine.Client;
using PayrollEngine.Client.Model;
using PayrollEngine.WebApp.Shared;

namespace PayrollEngine.WebApp.Presentation.BackendService;

public class EmployeeCaseChangeValueBackendService(UserSession userSession,
    PayrollHttpClient httpClient
    , Localizer localizer)
    : CaseChangeValueBackendServiceBase(userSession, httpClient, localizer)
{
    protected override void SetupReadQuery(PayrollCaseChangeQuery query, IDictionary<string, object> parameters = null)
    {
        base.SetupReadQuery(query, parameters);
        query.CaseType = CaseType.Employee;
        query.EmployeeId = UserSession.Employee.Id;

        // division
        if (UserSession.Payroll != null)
        {
            query.DivisionId = UserSession.Payroll.DivisionId;
        }
    }
}