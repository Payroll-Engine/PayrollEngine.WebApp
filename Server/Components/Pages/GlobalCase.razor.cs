using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using PayrollEngine.Client.Model;
using PayrollEngine.Client.Service;
using PayrollEngine.WebApp.Server.Components.Shared;

namespace PayrollEngine.WebApp.Server.Components.Pages;

public partial class GlobalCase()
    : NewCasePageBase(WorkingItems.TenantView | WorkingItems.PayrollView), ICaseValueProvider
{
    [Inject]
    private IGlobalCaseValueService GlobalCaseValueService { get; set; }

    protected override CaseType CaseType => CaseType.Global;
    protected override string ParentPageName => PageUrls.GlobalCases;
    protected override ICaseValueProvider CaseValueProvider => this;

    async Task<IEnumerable<CaseValueSetup>> ICaseValueProvider.GetCaseValuesAsync(CaseValueQuery query)
    {
        return await GlobalCaseValueService.QueryAsync<CaseValueSetup>(new(Tenant.Id), query);
    }
}