using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using PayrollEngine.WebApp.ViewModel;
using PayrollEngine.WebApp.Presentation;
using PayrollEngine.WebApp.Presentation.BackendService;
using PayrollEngine.WebApp.Presentation.Component;
using PayrollEngine.WebApp.Server.Shared;

namespace PayrollEngine.WebApp.Server.Pages;

public partial class SharedRegulations() : EditItemPageBase<RegulationShare, Query, Dialogs.RegulationShareDialog>(
    WorkingItems.None) 
{
    [Inject]
    private RegulationShareBackendService RegulationShareBackendService { get; set; }

    protected override string GridId => GetTenantGridId(GridIdentifiers.Payrolls);
    protected override IBackendService<RegulationShare, Query> BackendService => RegulationShareBackendService;
    protected override ItemCollection<RegulationShare> Items { get; } = new();
    protected override string GetLocalizedItemName(bool plural) =>
        plural ? Localizer.RegulationShare.RegulationShares : Localizer.RegulationShare.RegulationShare;

    protected override async Task<bool> SetupDialogParametersAsync<T>(DialogParameters parameters, ItemOperation operation, T item)
    {
        // culture
        parameters.Add("Culture", PageCulture);
        return await base.SetupDialogParametersAsync(parameters, operation, item);
    }
}