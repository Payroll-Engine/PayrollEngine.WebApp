using System.Linq;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using PayrollEngine.Client.Service;
using PayrollEngine.WebApp.Server.Dialogs;
using PayrollEngine.WebApp.ViewModel;
using System.Threading.Tasks;
using PayrollEngine.WebApp.Presentation;
using PayrollEngine.WebApp.Presentation.BackendService;
using PayrollEngine.WebApp.Server.Shared;

namespace PayrollEngine.WebApp.Server.Pages;

public partial class Payrolls
{
    [Inject]
    private PayrollBackendService PayrollBackendService { get; set; }
    [Inject]
    private IDivisionService DivisionService { get; set; }

    protected override string GridId => GetTenantGridId(GridIdentifiers.Payrolls);
    protected override IBackendService<Payroll, Query> BackendService => PayrollBackendService;
    protected override ItemCollection<Payroll> Items { get; } = new();

    public Payrolls() :
        base(WorkingItems.TenantChange)
    {
    }

    protected override async Task<bool> OnItemCommit(Payroll payroll, ItemOperation operation)
    {
        await PayrollBackendService.ApplyDivisionAsync(payroll);
        return await base.OnItemCommit(payroll, operation);
    }

    protected override async Task<bool> SetupDialogParametersAsync(DialogParameters parameters, ItemOperation operation)
    {
        // parameter division names
        var result = await DivisionService.QueryAsync<Division>(new(Tenant.Id));
        parameters.Add(nameof(PayrollDialog.DivisionNames), result.Select(x => x.Name).ToList());

        return await base.SetupDialogParametersAsync(parameters, operation);
    }
}