using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using PayrollEngine.Client.Model;
using PayrollEngine.Client.Service;
using PayrollEngine.WebApp.Server.Shared;

namespace PayrollEngine.WebApp.Server.Pages;

public partial class EmployeeCase() : NewCasePageBase(WorkingItems.TenantView | WorkingItems.PayrollView |
                                                      WorkingItems.EmployeeView), ICaseValueProvider
{
    [Inject]
    private IEmployeeCaseValueService EmployeeCaseValueService { get; set; }

    protected override CaseType CaseType => CaseType.Employee;
    protected override string ParentPageName => PageUrls.EmployeeCases;
    protected override ICaseValueProvider CaseValueProvider => this;

    async Task<IEnumerable<CaseValueSetup>> ICaseValueProvider.GetCaseValuesAsync(CaseValueQuery query)
    {
        if (Employee == null)
        {
            return Enumerable.Empty<CaseValueSetup>();
        }
        return await EmployeeCaseValueService.QueryAsync<CaseValueSetup>(new(Tenant.Id, Employee.Id), query);
    }
}