using Microsoft.AspNetCore.Components;
using PayrollEngine.WebApp.Presentation;
using PayrollEngine.WebApp.Presentation.BackendService;
using PayrollEngine.WebApp.Server.Shared;

namespace PayrollEngine.WebApp.Server.Pages;

public partial class Logs
{
    [Inject]
    private LogBackendService LogBackendService { get; set; }

    protected override string GridId => GetTenantGridId(GridIdentifiers.Logs);
    protected override IBackendService<ViewModel.Log, Query> BackendService => LogBackendService;
    protected override ItemCollection<ViewModel.Log> Items { get; } = new();
    protected override string GetLocalizedItemName(bool plural) => 
        plural ? Localizer.Log.Logs : Localizer.Log.Log;

    public Logs() :
        base(WorkingItems.TenantChange)
    {
    }
}