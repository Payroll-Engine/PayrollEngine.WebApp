using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using PayrollEngine.Client.QueryExpression;
using PayrollEngine.Client.Service;
using PayrollEngine.Client.Service.Api;
using PayrollEngine.WebApp.Shared;
using PayrollEngine.WebApp.ViewModel;

namespace PayrollEngine.WebApp.Presentation.BackendService;

public class PayrunPayrunJobBackendService : BackendServiceBase<PayrunJobService, PayrollServiceContext, PayrunJob, Query>
{
    public PayrunPayrunJobBackendService(UserSession userSession, IConfiguration configuration, Localizer localizer) :
        base(userSession, configuration, localizer)
    {
    }

    /// <summary>The current request context</summary>
    protected override PayrollServiceContext CreateServiceContext(IDictionary<string, object> parameters = null)
    {
        if (UserSession.Tenant != null && UserSession.Payroll != null)
        {
            return new(UserSession.Tenant.Id, UserSession.Payroll.Id);
        }
        return null;
    }

    /// <summary>Create the backend service</summary>
    protected override PayrunJobService CreateService() =>
        new(HttpClient);

    protected override void SetupReadQuery(Query query, IDictionary<string, object> parameters = null)
    {
        if (parameters == null || !parameters.ContainsKey(nameof(PayrunJob.PayrunId)))
        {
            throw new PayrollException("Missing payrun id on payrun job query");
        }
        var payrunId = parameters[nameof(PayrunJob.PayrunId)];
        if (payrunId is not int intPayrunId || intPayrunId <= 0)
        {
            throw new PayrollException("Invalid payrun id on payrun job query");
        }

        var filter = new Equals(nameof(PayrunJob.PayrunId), intPayrunId);
        query.Filter = query.HasFilter() ? new Filter(query.Filter).And(filter) : filter;
        base.SetupReadQuery(query, parameters);
    }
}