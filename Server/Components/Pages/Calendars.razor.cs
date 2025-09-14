using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using PayrollEngine.WebApp.ViewModel;
using PayrollEngine.WebApp.Presentation;
using PayrollEngine.WebApp.Presentation.Component;
using PayrollEngine.WebApp.Server.Components.Shared;
using PayrollEngine.WebApp.Presentation.BackendService;

namespace PayrollEngine.WebApp.Server.Components.Pages;

public partial class Calendars() : EditItemPageBase<Calendar, Query, Dialogs.CalendarDialog>(WorkingItems.TenantChange)
{
    [Inject]
    private CalendarBackendService CalendarBackendService { get; set; }
    protected override string GridId => GetTenantGridId(GridIdentifiers.Calendars);
    protected override IBackendService<Calendar, Query> BackendService => CalendarBackendService;
    protected override ItemCollection<Calendar> Items { get; } = new();
    protected override string GetLocalizedItemName(bool plural) =>
        plural ? Localizer.Calendar.Calendars : Localizer.Calendar.Calendar;

    protected override async Task<bool> SetupDialogParametersAsync(DialogParameters parameters, ItemOperation operation, Calendar item)
    {
        // culture parameter
        parameters.Add(nameof(Dialogs.CalendarDialog.CultureInfo), PageCulture);
        return await base.SetupDialogParametersAsync(parameters, operation, item);
    }
}