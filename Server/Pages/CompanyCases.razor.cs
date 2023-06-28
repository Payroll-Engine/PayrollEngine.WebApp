using PayrollEngine.Client.Model;
using Microsoft.AspNetCore.Components;
using PayrollEngine.WebApp.Server.Shared;
using PayrollEngine.WebApp.Presentation.BackendService;
using PayrollEngine.WebApp.Presentation;

namespace PayrollEngine.WebApp.Server.Pages;

public partial class CompanyCases
{
    [Inject]
    private CompanyCaseChangeValueBackendService CaseValueService { get; set; }
    [Inject]
    private CompanyCaseDocumentBackendService CaseDocumentService { get; set; }

    public CompanyCases() :
        base(WorkingItems.TenantChange | WorkingItems.PayrollChange)
    {
    }

    // mandatory
    protected override CaseType CaseType => CaseType.Company;
    protected override string NewCasePageName => PageUrls.CompanyCase;
    protected override string PageTitle => Localizer.Case.CompanyCases;
    protected override string GridId => GetTenantGridId(GridIdentifiers.CompanyCaseChangeValues);

    protected override IBackendService<ViewModel.CaseChangeCaseValue, PayrollCaseChangeQuery> CaseValueBackendService => CaseValueService;
    protected override IBackendService<CaseDocument, Query> CaseDocumentBackendService => CaseDocumentService;
}