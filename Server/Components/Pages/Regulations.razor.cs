using Microsoft.AspNetCore.Components;
using PayrollEngine.WebApp.Presentation;
using PayrollEngine.WebApp.Presentation.BackendService;
using PayrollEngine.WebApp.Presentation.Component;
using PayrollEngine.WebApp.Server.Components.Shared;

namespace PayrollEngine.WebApp.Server.Components.Pages;

public partial class Regulations() : EditItemPageBase<ViewModel.Regulation, Query, Dialogs.RegulationDialog>(
    WorkingItems.TenantChange) 
{
    [Inject]
    private RegulationBackendService RegulationBackendService { get; set; }

    protected override string GridId => GetTenantGridId(GridIdentifiers.Regulations);
    protected override IBackendService<ViewModel.Regulation, Query> BackendService => RegulationBackendService;
    protected override ItemCollection<ViewModel.Regulation> Items { get; } = new();
    protected override string GetLocalizedItemName(bool plural) => 
        plural ? Localizer.Regulation.Regulations : Localizer.Regulation.Regulation;
}