using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using PayrollEngine.Client.Model;
using PayrollEngine.Client.Service;
using PayrollEngine.WebApp.Server.Components.Shared;

namespace PayrollEngine.WebApp.Server.Components.Pages;

public partial class NationalCase() : NewCasePageBase(WorkingItems.TenantView | WorkingItems.PayrollView),
    ICaseValueProvider
{
    [Inject]
    private INationalCaseValueService NationalCaseValueService { get; set; }

    protected override CaseType CaseType => CaseType.National;
    protected override string ParentPageName => PageUrls.NationalCases;
    protected override ICaseValueProvider CaseValueProvider => this;

    async Task<IEnumerable<CaseValueSetup>> ICaseValueProvider.GetCaseValuesAsync(CaseValueQuery query)
    {
        return await NationalCaseValueService.QueryAsync<CaseValueSetup>(new(Tenant.Id), query);
    }
}