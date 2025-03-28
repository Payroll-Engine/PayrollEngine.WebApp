﻿using System.Collections.Generic;
using PayrollEngine.Client;
using PayrollEngine.Client.QueryExpression;
using PayrollEngine.Client.Service;
using PayrollEngine.Client.Service.Api;
using PayrollEngine.WebApp.Shared;
using PayrollEngine.WebApp.ViewModel;

namespace PayrollEngine.WebApp.Presentation.BackendService;

public class PayrunPayrunJobBackendService(UserSession userSession, 
    PayrollHttpClient httpClient, 
    ILocalizerService localizerService)
    : BackendServiceBase<PayrunJobService, PayrollServiceContext, PayrunJob, Query>(
        userSession, httpClient, localizerService)
{
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
        if (parameters == null || !parameters.TryGetValue(nameof(PayrunJob.PayrunId), out var payrunId))
        {
            throw new PayrollException("Missing payrun id on payrun job query.");
        }

        if (payrunId is not int intPayrunId || intPayrunId <= 0)
        {
            throw new PayrollException("Invalid payrun id on payrun job query.");
        }

        var filter = new Equals(nameof(PayrunJob.PayrunId), intPayrunId);
        query.Filter = query.HasFilter() ? new Filter(query.Filter).And(filter) : filter;
        base.SetupReadQuery(query, parameters);
    }
}