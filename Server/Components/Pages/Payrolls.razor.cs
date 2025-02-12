using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using PayrollEngine.Client.Service;
using PayrollEngine.WebApp.ViewModel;
using PayrollEngine.WebApp.Presentation;
using PayrollEngine.WebApp.Presentation.Component;
using PayrollEngine.WebApp.Server.Components.Shared;
using PayrollEngine.WebApp.Server.Components.Dialogs;
using PayrollEngine.WebApp.Presentation.BackendService;

namespace PayrollEngine.WebApp.Server.Components.Pages;

public partial class Payrolls() : EditItemPageBase<Payroll, Query, PayrollDialog>(WorkingItems.TenantChange)
{
    [Inject]
    private PayrollBackendService PayrollBackendService { get; set; }
    [Inject]
    private IDivisionService DivisionService { get; set; }

    protected override string GridId => GetTenantGridId(GridIdentifiers.Payrolls);
    protected override IBackendService<Payroll, Query> BackendService => PayrollBackendService;
    protected override ItemCollection<Payroll> Items { get; } = new();
    protected override string GetLocalizedItemName(bool plural) =>
        plural ? Localizer.Payroll.Payrolls : Localizer.Payroll.Payroll;

    protected override async Task<bool> OnItemCommit(Payroll payroll)
    {
        await PayrollBackendService.ApplyDivisionAsync(payroll);
        return await base.OnItemCommit(payroll);
    }

    protected override async Task<bool> SetupDialogParametersAsync(DialogParameters parameters, ItemOperation operation, Payroll item)
    {
        // parameter division names
        var result = await DivisionService.QueryAsync<Division>(new(Tenant.Id));
        parameters.Add(nameof(PayrollDialog.DivisionNames), result.Select(x => x.Name).ToList());

        return await base.SetupDialogParametersAsync(parameters, operation, item);
    }
}