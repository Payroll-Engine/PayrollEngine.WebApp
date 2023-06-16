using Microsoft.AspNetCore.Components;
using PayrollEngine.WebApp.Presentation;
using PayrollEngine.WebApp.Presentation.BackendService;
using PayrollEngine.WebApp.Server.Shared;
using Calendar = PayrollEngine.WebApp.ViewModel.Calendar;

namespace PayrollEngine.WebApp.Server.Pages;

public partial class Calendars
{
    [Inject]
    private CalendarBackendService CalendarBackendService { get; set; }

    protected override string GridId => GetTenantGridId(GridIdentifiers.Calendars);
    protected override IBackendService<Calendar, Query> BackendService => CalendarBackendService;
    protected override ItemCollection<Calendar> Items { get; } = new();

    public Calendars() :
        base(WorkingItems.TenantChange)
    {
    }
}