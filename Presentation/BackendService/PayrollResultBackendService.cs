using System.Collections.Generic;
using System.Net.Http;
using PayrollEngine.Client;
using PayrollEngine.Client.QueryExpression;
using PayrollEngine.Client.Service;
using PayrollEngine.Client.Service.Api;
using PayrollEngine.WebApp.Shared;

namespace PayrollEngine.WebApp.Presentation.BackendService;

public class PayrollResultBackendService(
    UserSession userSession,
    HttpClientHandler httpClientHandler,
    PayrollHttpConfiguration configuration,
    Localizer localizer)
    : BackendServiceBase<PayrollResultValueService, PayrollResultValueServiceContext, ViewModel.PayrollResultValue,
        Query>(userSession, httpClientHandler, configuration, localizer)
{
    /// <summary>The current request context</summary>
    protected override PayrollResultValueServiceContext CreateServiceContext(IDictionary<string, object> parameters = null) =>
        UserSession.Tenant != null
            ? new PayrollResultValueServiceContext(UserSession.Tenant.Id, payrollId: UserSession.Payroll?.Id,
                employeeId: UserSession.Employee?.Id)
            : null;


    /// <summary>Create the backend service</summary>
    protected override PayrollResultValueService CreateService() =>
        new(HttpClient);

    protected override void SetupReadQuery(Query query, IDictionary<string, object> parameters = null)
    {
        // payrun
        if (parameters == null || !parameters.TryGetValue(nameof(ViewModel.PayrollResultValue.PayrunId), out var payrunId))
        {
            throw new PayrollException("Missing payrun id on payroll result query");
        }

        if (payrunId is not int intPayrunId || intPayrunId <= 0)
        {
            throw new PayrollException("Invalid payrun id on payroll result query");
        }

        // employee
        var employeeId = UserSession.Employee?.Id;
        if (!employeeId.HasValue || employeeId.Value == default)
        {
            throw new PayrollException("Missing employee id on payroll result query");
        }

        var filter = new Equals(nameof(ViewModel.PayrollResultValue.PayrunId), intPayrunId);
        query.Filter = query.HasFilter() ? new Filter(query.Filter).And(filter) : filter;
        base.SetupReadQuery(query, parameters);
    }
}