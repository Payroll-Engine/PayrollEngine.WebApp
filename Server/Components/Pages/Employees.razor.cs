using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using PayrollEngine.Client.Model;
using PayrollEngine.WebApp.Presentation;
using PayrollEngine.WebApp.Presentation.Component;
using PayrollEngine.WebApp.Server.Components.Shared;
using PayrollEngine.WebApp.Server.Components.Dialogs;
using PayrollEngine.WebApp.Presentation.BackendService;

namespace PayrollEngine.WebApp.Server.Components.Pages;

public partial class Employees() : EditItemPageBase<ViewModel.Employee, DivisionQuery, EmployeeDialog>(WorkingItems.TenantChange, useCalendar: true)
{
    [Inject]
    private EmployeeBackendService EmployeeBackendService { get; set; }
    [Inject]
    private DivisionBackendService DivisionService { get; set; }

    protected override string GridId => GetTenantGridId(GridIdentifiers.Employees);
    protected override IBackendService<ViewModel.Employee, DivisionQuery> BackendService => EmployeeBackendService;
    protected override ItemCollection<ViewModel.Employee> Items { get; } = new();
    protected override string GetLocalizedItemName(bool plural) =>
        plural ? Localizer.Employee.Employees : Localizer.Employee.Employee;

    protected override async Task<bool> OnItemCommit(ViewModel.Employee employee)
    {
        return await base.OnItemCommit(employee);
    }

    protected override async Task<bool> SetupDialogParametersAsync(DialogParameters parameters, ItemOperation operation, ViewModel.Employee item)
    {
        // parameter divisions
        var result = await DivisionService.QueryAsync();
        parameters.Add(nameof(EmployeeDialog.Divisions), result.Items.ToList());

        return await base.SetupDialogParametersAsync(parameters, operation, item);
    }
}