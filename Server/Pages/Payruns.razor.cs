using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using PayrollEngine.Client.Model;
using PayrollEngine.Client.Service;
using PayrollEngine.WebApp.Presentation;
using PayrollEngine.WebApp.Presentation.BackendService;
using PayrollEngine.WebApp.Server.Dialogs;
using PayrollEngine.WebApp.Server.Shared;
using Payrun = PayrollEngine.WebApp.ViewModel.Payrun;

namespace PayrollEngine.WebApp.Server.Pages;

public partial class Payruns
{
    [Inject]
    private PayrunBackendService PayrunBackendService { get; set; }
    [Inject]
    private IPayrollService PayrollService { get; set; }

    protected override string GridId => GetTenantGridId(GridIdentifiers.Payruns);
    protected override IBackendService<Payrun, Query> BackendService => PayrunBackendService;
    protected override ItemCollection<Payrun> Items { get; } = new();

    public Payruns() :
        base(WorkingItems.TenantChange)
    {
    }

    protected override async Task<bool> OnItemCommit(Payrun payroll, ItemOperation operation)
    {
        await PayrunBackendService.ApplyPayrollAsync(payroll);
        return await base.OnItemCommit(payroll, operation);
    }

    protected override async Task<bool> SetupDialogParametersAsync(DialogParameters parameters, ItemOperation operation)
    {
        // parameter payroll names
        var result = await PayrollService.QueryAsync<Payroll>(new(Tenant.Id));
        parameters.Add(nameof(PayrunDialog.Tenant), Tenant);
        parameters.Add(nameof(PayrunDialog.PayrollNames), result.Select(x => x.Name).ToList());

        return await base.SetupDialogParametersAsync(parameters, operation);
    }
}