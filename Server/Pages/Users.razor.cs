using Microsoft.AspNetCore.Components;
using PayrollEngine.WebApp.Presentation;
using PayrollEngine.WebApp.Presentation.BackendService;
using PayrollEngine.WebApp.Server.Shared;

namespace PayrollEngine.WebApp.Server.Pages;

public partial class Users
{
    [Inject]
    protected UserBackendService UserBackendService { get; set; }

    protected override string GridId => GetTenantGridId(GridIdentifiers.Users);
    protected override IBackendService<ViewModel.User, Query> BackendService => UserBackendService;
    protected override ItemCollection<ViewModel.User> Items { get; } = new();

    public Users() :
        base(WorkingItems.TenantChange)
    {
    }
}