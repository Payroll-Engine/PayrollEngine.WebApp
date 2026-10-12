using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using PayrollEngine.Client.Service;
using PayrollEngine.WebApp.ViewModel;
using PayrollEngine.WebApp.Presentation;
using PayrollEngine.WebApp.Presentation.Component;
using PayrollEngine.WebApp.Server.Components.Shared;
using PayrollEngine.WebApp.Presentation.BackendService;
using Calendar = PayrollEngine.WebApp.ViewModel.Calendar;

namespace PayrollEngine.WebApp.Server.Components.Pages;

public partial class Calendars() : EditItemPageBase<Calendar, Query, Dialogs.CalendarDialog>(WorkingItems.TenantChange)
{
    [Inject]
    private CalendarBackendService CalendarBackendService { get; set; }
    [Inject]
    private IDivisionService DivisionService { get; set; }
    [Inject]
    private IEmployeeService EmployeeService { get; set; }
    [Inject]
    private IPayrunJobService PayrunJobService { get; set; }
    protected override string GridId => GetTenantGridId(GridIdentifiers.Calendars);
    protected override IBackendService<Calendar, Query> BackendService => CalendarBackendService;
    protected override ItemCollection<Calendar> Items { get; } = new();
    protected override string GetLocalizedItemName(bool plural) =>
        plural ? Localizer.Calendar.Calendars : Localizer.Calendar.Calendar;

    protected override async Task<bool> SetupDialogParametersAsync(DialogParameters parameters, ItemOperation operation, Calendar item)
    {
        // culture parameter
        parameters.Add(nameof(Dialogs.CalendarDialog.CultureInfo), PageCulture);
        // the period anchor date defines the week and bi-week period sequence of existing payrun jobs
        if (operation == ItemOperation.Edit && item != null)
        {
            parameters.Add(nameof(Dialogs.CalendarDialog.PeriodAnchorDateLocked), await IsCalendarInUseAsync(item.Name));
        }
        return await base.SetupDialogParametersAsync(parameters, operation, item);
    }

    private async Task<bool> IsCalendarInUseAsync(string calendarName)
    {
        if (Tenant == null || string.IsNullOrWhiteSpace(calendarName))
        {
            return false;
        }

        var context = new TenantServiceContext(Tenant.Id);
        var divisions = await DivisionService.QueryAsync<Client.Model.Division>(context);
        foreach (var division in divisions)
        {
            var divisionCalendar = string.IsNullOrWhiteSpace(division.Calendar) ? Tenant.Calendar : division.Calendar;
            if (string.Equals(divisionCalendar, calendarName) &&
                await PayrunJobService.QueryCountAsync(context,
                    new() { Filter = $"{nameof(Client.Model.PayrunJob.DivisionId)} eq {division.Id}" }) > 0)
            {
                return true;
            }
        }

        var employeeCount = await EmployeeService.QueryCountAsync(context,
            new() { Filter = $"{nameof(Client.Model.Employee.Calendar)} eq '{calendarName.Replace("'", "''")}'" });
        return employeeCount > 0 && await PayrunJobService.QueryCountAsync(context) > 0;
    }
}