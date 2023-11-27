using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using PayrollEngine.Client.Model;
using PayrollEngine.Client.Service;
using PayrollEngine.WebApp.Server.Shared;

namespace PayrollEngine.WebApp.Server.Pages;

public partial class CompanyCase() : NewCasePageBase(WorkingItems.TenantView | WorkingItems.PayrollView),
    ICaseValueProvider
{
    [Inject]
    private ICompanyCaseValueService CompanyCaseValueService { get; set; }

    protected override CaseType CaseType => CaseType.Company;
    protected override string ParentPageName => PageUrls.CompanyCases;
    protected override ICaseValueProvider CaseValueProvider => this;

    async Task<IEnumerable<CaseValueSetup>> ICaseValueProvider.GetCaseValuesAsync(CaseValueQuery query)
    {
        return await CompanyCaseValueService.QueryAsync<CaseValueSetup>(new(Tenant.Id), query);
    }
}