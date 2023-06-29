using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using PayrollEngine.WebApp.ViewModel;
using PayrollEngine.WebApp.Presentation;
using PayrollEngine.WebApp.Presentation.BackendService;
using PayrollEngine.WebApp.Server.Shared;

namespace PayrollEngine.WebApp.Server.Pages;

public partial class SharedRegulations
{
    [Inject]
    private RegulationShareBackendService RegulationShareBackendService { get; set; }

    protected override string GridId => GetTenantGridId(GridIdentifiers.Payrolls);
    protected override IBackendService<RegulationShare, Query> BackendService => RegulationShareBackendService;
    protected override ItemCollection<RegulationShare> Items { get; } = new();

    public SharedRegulations() :
        base(WorkingItems.None)
    {
    }

    protected override async Task<bool> SetupDialogParametersAsync(DialogParameters parameters, ItemOperation operation)
    {
        // culture
        parameters.Add("Culture", Culture);
        return await base.SetupDialogParametersAsync(parameters, operation);
    }
}