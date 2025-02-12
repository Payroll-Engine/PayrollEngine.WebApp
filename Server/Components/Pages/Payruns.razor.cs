using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using PayrollEngine.Client.Model;
using PayrollEngine.Client.Service;
using PayrollEngine.WebApp.Presentation;
using PayrollEngine.WebApp.Presentation.BackendService;
using PayrollEngine.WebApp.Server.Components.Dialogs;
using PayrollEngine.WebApp.Server.Components.Shared;
using Payrun = PayrollEngine.WebApp.ViewModel.Payrun;

namespace PayrollEngine.WebApp.Server.Components.Pages;

public partial class Payruns
{
    [Inject]
    private PayrunBackendService PayrunBackendService { get; set; }
    [Inject]
    private IPayrollService PayrollService { get; set; }

    protected override string GridId => GetTenantGridId(GridIdentifiers.Payruns);
    protected override IBackendService<Payrun, Query> BackendService => PayrunBackendService;
    protected override ItemCollection<Payrun> Items { get; } = new();
    protected override string GetLocalizedItemName(bool plural) => 
        plural ? Localizer.Payrun.Payruns : Localizer.Payrun.Payrun;

    // ReSharper disable once ConvertToPrimaryConstructor
    public Payruns() :
        base(WorkingItems.TenantChange)
    {
    }

    protected override async Task<bool> OnItemCommit(Payrun payroll)
    {
        await PayrunBackendService.ApplyPayrollAsync(payroll);
        return await base.OnItemCommit(payroll);
    }

    protected override async Task<bool> SetupDialogParametersAsync(DialogParameters parameters, ItemOperation operation, Payrun item)
    {
        // parameter payroll names
        var result = await PayrollService.QueryAsync<Payroll>(new(Tenant.Id));
        parameters.Add(nameof(PayrunDialog.Tenant), Tenant);
        parameters.Add(nameof(PayrunDialog.PayrollNames), result.Select(x => x.Name).ToList());

       return await base.SetupDialogParametersAsync(parameters, operation, item);
    }
}