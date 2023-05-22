using PayrollEngine.Client.Model;
using PayrollEngine.Client.Service;
using Microsoft.AspNetCore.Components;
using PayrollEngine.WebApp.Presentation;
using PayrollEngine.WebApp.Presentation.BackendService;
using PayrollEngine.WebApp.Server.Shared;

namespace PayrollEngine.WebApp.Server.Pages;

public partial class EmployeeCases
{
    [Inject]
    protected IEmployeeCaseValueService EmployeeCaseValueService { get; set; }
    [Inject]
    protected IEmployeeCaseChangeService EmployeeCaseChangeService { get; set; }
    [Inject]
    protected EmployeeCaseChangeValueBackendService CaseValueService { get; set; }
    [Inject]
    protected EmployeeCaseDocumentBackendService CaseDocumentService { get; set; }

    public EmployeeCases() :
        base(WorkingItems.TenantChange | WorkingItems.PayrollChange | WorkingItems.EmployeeChange)
    {
    }

    protected override CaseType CaseType => CaseType.Employee;
    protected override string NewCasePageName => PageUrls.EmployeeCase;
    protected override string PageTitle => "Employee Cases";
    protected override string GridId => GetTenantGridId(GridIdentifiers.EmployeeCaseChangeValues);

    protected override IBackendService<ViewModel.CaseChangeCaseValue, PayrollCaseChangeQuery> CaseValueBackendService => CaseValueService;
    protected override IBackendService<CaseDocument, Query> CaseDocumentBackendService => CaseDocumentService;

    protected override PayrollCaseChangeQuery NewCaseChangeQuery()
    {
        if (Employee == null)
        {
            return new();
        }

        var query = base.NewCaseChangeQuery();
        query.EmployeeId = Employee.Id;
        return query;
    }
}