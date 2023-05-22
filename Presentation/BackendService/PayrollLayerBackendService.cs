using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using PayrollEngine.Client.Service;
using PayrollEngine.Client.Service.Api;
using PayrollEngine.WebApp.ViewModel;

namespace PayrollEngine.WebApp.Presentation.BackendService;

public class PayrollLayerBackendService : BackendServiceBase<PayrollLayerService, PayrollServiceContext, PayrollLayer, Query>
{
    protected IPayrollService PayrollService { get; set; }

    public PayrollLayerBackendService(UserSession userSession, IConfiguration configuration,
        IPayrollService payrollService) :
        base(userSession, configuration)
    {
        PayrollService = payrollService ?? throw new ArgumentNullException(nameof(payrollService));
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
    protected override PayrollLayerService CreateService(IDictionary<string, object> parameters = null) =>
        new(HttpClient);
}
