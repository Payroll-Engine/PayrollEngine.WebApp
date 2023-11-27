using Microsoft.AspNetCore.Components;
using PayrollEngine.WebApp.Presentation;
using PayrollEngine.WebApp.Presentation.BackendService;
using PayrollEngine.WebApp.Presentation.Component;
using PayrollEngine.WebApp.Server.Shared;
using Calendar = PayrollEngine.WebApp.ViewModel.Calendar;

namespace PayrollEngine.WebApp.Server.Pages;

public partial class Calendars() : EditItemPageBase<Calendar, Query, Dialogs.CalendarDialog>(WorkingItems.TenantChange) 
{
    [Inject]
    private CalendarBackendService CalendarBackendService { get; set; }

    protected override string GridId => GetTenantGridId(GridIdentifiers.Calendars);
    protected override IBackendService<Calendar, Query> BackendService => CalendarBackendService;
    protected override ItemCollection<Calendar> Items { get; } = new();
    protected override string GetLocalizedItemName(bool plural) => 
        plural ? Localizer.Calendar.Calendars : Localizer.Calendar.Calendar;
}