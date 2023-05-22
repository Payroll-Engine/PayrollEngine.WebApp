using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using PayrollEngine.Client.QueryExpression;
using PayrollEngine.Client.Service;
using PayrollEngine.Client.Service.Api;

namespace PayrollEngine.WebApp.Presentation.BackendService;

public class PayrollResultBackendService : BackendServiceBase<PayrollResultValueService, PayrollResultValueServiceContext, ViewModel.PayrollResultValue, Query>
{
    public PayrollResultBackendService(UserSession userSession, IConfiguration configuration) :
        base(userSession, configuration)
    {
    }

    /// <summary>The current request context</summary>
    protected override PayrollResultValueServiceContext CreateServiceContext(IDictionary<string, object> parameters = null) =>
        UserSession.Tenant != null
            ? new PayrollResultValueServiceContext(UserSession.Tenant.Id, payrollId: UserSession.Payroll?.Id,
                employeeId: UserSession.Employee?.Id)
            : null;


    /// <summary>Create the backend service</summary>
    protected override PayrollResultValueService CreateService(IDictionary<string, object> parameters = null) =>
        new(HttpClient);

    protected override void SetupReadQuery(PayrollResultValueServiceContext context, Query query, IDictionary<string, object> parameters = null)
    {
        if (parameters == null || !parameters.ContainsKey(nameof(ViewModel.PayrollResultValue.PayrunId)))
        {
            throw new PayrollException("Missing payrun id on payroll result query");
        }
        var payrunId = parameters[nameof(ViewModel.PayrollResultValue.PayrunId)];
        if (payrunId is not int intPayrunId || intPayrunId <= 0)
        {
            throw new PayrollException("Invalid payrun id on payroll result query");
        }

        var filter = new Equals(nameof(ViewModel.PayrollResultValue.PayrunId), intPayrunId);
        query.Filter = query.HasFilter() ? new Filter(query.Filter).And(filter) : filter;
        base.SetupReadQuery(context, query, parameters);
    }
}