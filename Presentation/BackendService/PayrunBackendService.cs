using System;
using System.Collections.Generic;
using System.Net.Http;
using PayrollEngine.Client;
using PayrollEngine.Client.Model;
using PayrollEngine.Client.Service;
using PayrollEngine.Client.Service.Api;
using PayrollEngine.WebApp.Shared;
using Task = System.Threading.Tasks.Task;

namespace PayrollEngine.WebApp.Presentation.BackendService;

public class PayrunBackendService : BackendServiceBase<PayrunService, TenantServiceContext, ViewModel.Payrun, Query>
{
    private IPayrollService PayrollService { get; }

    public PayrunBackendService(UserSession userSession, HttpClientHandler httpClientHandler,
        PayrollHttpConfiguration configuration, Localizer localizer, IPayrollService payrollService) :
        base(userSession, httpClientHandler, configuration, localizer)
    {
        PayrollService = payrollService ?? throw new ArgumentNullException(nameof(payrollService));
    }

    /// <summary>The current request context</summary>
    protected override TenantServiceContext CreateServiceContext(IDictionary<string, object> parameters = null) =>
        UserSession.Tenant != null ? new TenantServiceContext(UserSession.Tenant.Id) : null;

    /// <summary>Create the backend service</summary>
    protected override PayrunService CreateService() =>
        new(HttpClient);

    public async Task ApplyPayrollAsync(ViewModel.Payrun payrun) =>
        await ApplyPayrollAsync(new[] { payrun });

    private async Task ApplyPayrollAsync(IEnumerable<ViewModel.Payrun> payruns)
    {
        foreach (var payrun in payruns)
        {
            // priority 1: ensure payroll id
            if (!string.IsNullOrWhiteSpace(payrun.PayrollName))
            {
                var payroll = await PayrollService.GetAsync<Payroll>(new(UserSession.Tenant.Id), payrun.PayrollName);
                if (payroll != null)
                {
                    payrun.PayrollId = payroll.Id;
                }
            }
            // priority 2: ensure payroll name
            else if (payrun.PayrollId != 0)
            {
                var payroll = await PayrollService.GetAsync<Payroll>(new(UserSession.Tenant.Id), payrun.PayrollId);
                if (payroll != null)
                {
                    payrun.PayrollName = payroll.Name;
                }
            }
        }
    }

    protected override async Task OnItemsReadAsync(List<ViewModel.Payrun> payruns)
    {
        await ApplyPayrollAsync(payruns);
        await base.OnItemsReadAsync(payruns);
    }
}