using Microsoft.AspNetCore.Components;
using PayrollEngine.WebApp.Presentation;
using PayrollEngine.WebApp.Presentation.BackendService;
using PayrollEngine.WebApp.Server.Shared;
using PayrollEngine.WebApp.ViewModel;

namespace PayrollEngine.WebApp.Server.Pages;

public partial class Tenants
{
    protected override string GridId => GetTenantGridId(GridIdentifiers.Tenants);

    [Inject]
    protected TenantBackendService TenantBackendService { get; set; }
    protected override IBackendService<Tenant, Query> BackendService => TenantBackendService;
    protected override ItemCollection<Tenant> Items => Session.Tenants;

    public Tenants() :
        base(WorkingItems.None)
    {
    }
}