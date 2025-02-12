using Microsoft.AspNetCore.Components;
using PayrollEngine.WebApp.Presentation;
using PayrollEngine.WebApp.Presentation.BackendService;
using PayrollEngine.WebApp.Presentation.Component;
using PayrollEngine.WebApp.Server.Components.Shared;
using Division = PayrollEngine.WebApp.ViewModel.Division;

namespace PayrollEngine.WebApp.Server.Components.Pages;

public partial class Divisions() : EditItemPageBase<Division, Query, Dialogs.DivisionDialog>(
    WorkingItems.TenantChange, useCalendar: true) 
{
    [Inject]
    private DivisionBackendService DivisionBackendService { get; set; }

    protected override string GridId => GetTenantGridId(GridIdentifiers.Divisions);
    protected override IBackendService<Division, Query> BackendService => DivisionBackendService;
    protected override ItemCollection<Division> Items { get; } = new();
    protected override string GetLocalizedItemName(bool plural) => 
        plural ? Localizer.Division.Divisions : Localizer.Division.Division;
}