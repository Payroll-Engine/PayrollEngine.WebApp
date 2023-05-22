﻿using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using PayrollEngine.Client.Service;
using PayrollEngine.Client.Service.Api;
using PayrollEngine.WebApp.ViewModel;
using Task = System.Threading.Tasks.Task;

namespace PayrollEngine.WebApp.Presentation.BackendService;

public class PayrollBackendService : BackendServiceBase<PayrollService, TenantServiceContext, Payroll, Query>
{
    protected IDivisionService DivisionService { get; set; }

    public PayrollBackendService(UserSession userSession, IConfiguration configuration, IDivisionService divisionService) :
        base(userSession, configuration)
    {
        DivisionService = divisionService ?? throw new ArgumentNullException(nameof(divisionService));
    }

    /// <summary>The current request context</summary>
    protected override TenantServiceContext CreateServiceContext(IDictionary<string, object> parameters = null) =>
        UserSession.Tenant != null ? new TenantServiceContext(UserSession.Tenant.Id) : null;

    /// <summary>Create the backend service</summary>
    protected override PayrollService CreateService(IDictionary<string, object> parameters = null) =>
        new(HttpClient);

    public async Task ApplyDivisionAsync(Payroll payroll) =>
        await ApplyDivisionAsync(new[] { payroll });

    public async Task ApplyDivisionAsync(IEnumerable<Payroll> payrolls)
    {
        foreach (var payroll in payrolls)
        {
            // priority 1: ensure division id
            if (!string.IsNullOrWhiteSpace(payroll.DivisionName))
            {
                var division = await DivisionService.GetAsync<Division>(new(UserSession.Tenant.Id), payroll.DivisionName);
                if (division != null)
                {
                    payroll.DivisionId = division.Id;
                }
            }
            // priority 2: ensure division name
            else if (payroll.DivisionId != 0)
            {
                var division = await DivisionService.GetAsync<Division>(new(UserSession.Tenant.Id), payroll.DivisionId);
                if (division != null)
                {
                    payroll.DivisionName = division.Name;
                }
            }
        }
    }

    protected override async Task OnItemsReadAsync(List<Payroll> payrolls, IDictionary<string, object> parameters = null)
    {
        await ApplyDivisionAsync(payrolls);
        await base.OnItemsReadAsync(payrolls, parameters);
    }
}