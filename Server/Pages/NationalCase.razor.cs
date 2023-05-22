using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using PayrollEngine.Client.Model;
using PayrollEngine.Client.Service;
using PayrollEngine.WebApp.Server.Shared;

namespace PayrollEngine.WebApp.Server.Pages;

public partial class NationalCase : ICaseValueProvider
{
    public NationalCase() :
        base(WorkingItems.TenantView | WorkingItems.PayrollView)
    {
    }

    [Inject]
    protected INationalCaseValueService NationalCaseValueService { get; set; }

    protected override CaseType CaseType => CaseType.National;
    protected override string ParentPageName => PageUrls.NationalCases;
    protected override ICaseValueProvider CaseValueProvider => this;

    async Task<IEnumerable<CaseValueSetup>> ICaseValueProvider.GetCaseValuesAsync(CaseValueQuery query)
    {
        return await NationalCaseValueService.QueryAsync<CaseValueSetup>(new(Tenant.Id), query);
    }
}