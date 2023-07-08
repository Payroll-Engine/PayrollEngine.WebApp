using System.Linq;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using PayrollEngine.Client.Model;
using PayrollEngine.WebApp.Server.Dialogs;
using System.Threading.Tasks;
using PayrollEngine.WebApp.Presentation;
using PayrollEngine.WebApp.Presentation.BackendService;
using PayrollEngine.WebApp.Server.Shared;
using Employee = PayrollEngine.WebApp.ViewModel.Employee;

namespace PayrollEngine.WebApp.Server.Pages;

public partial class Employees
{
    [Inject]
    private EmployeeBackendService EmployeeBackendService { get; set; }
    [Inject]
    private DivisionBackendService DivisionService { get; set; }

    protected override string GridId => GetTenantGridId(GridIdentifiers.Employees);
    protected override IBackendService<Employee, DivisionQuery> BackendService => EmployeeBackendService;
    protected override ItemCollection<Employee> Items { get; } = new();
    protected override string GetLocalizedItemName(bool plural) => 
        plural ? Localizer.Employee.Employees : Localizer.Employee.Employee;

    public Employees() :
        base(WorkingItems.TenantChange)
    {
    }

    protected override async Task<bool> OnItemCommit(Employee employee)
    {
        return await base.OnItemCommit(employee);
    }

    protected override async Task<bool> SetupDialogParametersAsync(DialogParameters parameters, ItemOperation operation)
    {
        // parameter divisions
        var result = await DivisionService.QueryAsync();
        parameters.Add(nameof(EmployeeDialog.Divisions), result.Items.ToList());

        return await base.SetupDialogParametersAsync(parameters, operation);
    }
}