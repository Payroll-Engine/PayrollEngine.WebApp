using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using PayrollEngine.Client.Model;
using PayrollEngine.Client.Service;

namespace PayrollEngine.WebApp.Presentation.BackendService;

public class EmployeeCaseChangeValueBackendService : CaseChangeValueBackendService
{
    public EmployeeCaseChangeValueBackendService(UserSession userSession, IConfiguration configuration) :
        base(userSession, configuration)
    {
    }

    protected override void SetupReadQuery(PayrollServiceContext context, PayrollCaseChangeQuery query, IDictionary<string, object> parameters = null)
    {
        base.SetupReadQuery(context, query, parameters);
        query.CaseType = CaseType.Employee;
        query.EmployeeId = UserSession.Employee.Id;

        // division
        if (UserSession.Payroll != null)
        {
            query.DivisionId = UserSession.Payroll.DivisionId;
        }
    }
}