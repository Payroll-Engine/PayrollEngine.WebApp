using PayrollEngine.Client.Model;
using PayrollEngine.Client.Service;
using Microsoft.AspNetCore.Components;
using PayrollEngine.WebApp.Server.Shared;
using PayrollEngine.WebApp.Presentation.BackendService;
using PayrollEngine.WebApp.Presentation;

namespace PayrollEngine.WebApp.Server.Pages;

public partial class CompanyCases
{
    [Inject]
    protected ICompanyCaseValueService CompanyCaseValueService { get; set; }
    [Inject]
    protected ICompanyCaseChangeService CompanyCaseChangeService { get; set; }
    [Inject]
    protected CompanyCaseChangeValueBackendService CaseValueService { get; set; }
    [Inject]
    protected CompanyCaseDocumentBackendService CaseDocumentService { get; set; }

    public CompanyCases() :
        base(WorkingItems.TenantChange | WorkingItems.PayrollChange)
    {
    }

    // mandatory
    protected override CaseType CaseType => CaseType.Company;
    protected override string NewCasePageName => PageUrls.CompanyCase;
    protected override string PageTitle => "Company Cases";
    protected override string GridId => GetTenantGridId(GridIdentifiers.CompanyCaseChangeValues);

    protected override IBackendService<ViewModel.CaseChangeCaseValue, PayrollCaseChangeQuery> CaseValueBackendService => CaseValueService;
    protected override IBackendService<CaseDocument, Query> CaseDocumentBackendService => CaseDocumentService;
}