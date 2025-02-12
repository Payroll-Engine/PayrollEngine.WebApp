using Microsoft.AspNetCore.Components;
using PayrollEngine.Client.Model;
using PayrollEngine.WebApp.Presentation;
using PayrollEngine.WebApp.Presentation.BackendService;
using PayrollEngine.WebApp.Server.Components.Shared;

namespace PayrollEngine.WebApp.Server.Components.Pages;

public partial class CompanyCases() : CasesPageBase(WorkingItems.TenantChange |
    WorkingItems.PayrollChange) 
{
    [Inject]
    private CompanyCaseChangeValueBackendService CaseValueService { get; set; }
    [Inject]
    private CompanyCaseDocumentBackendService CaseDocumentService { get; set; }

    // mandatory
    protected override CaseType CaseType => CaseType.Company;
    protected override string NewCasePageName => PageUrls.CompanyCase;
    protected override string PageTitle => Localizer.Case.CompanyCases;
    protected override string GridId => GetTenantGridId(GridIdentifiers.CompanyCaseChangeValues);

    protected override IBackendService<ViewModel.CaseChangeCaseValue, PayrollCaseChangeQuery> CaseValueBackendService => CaseValueService;
    protected override IBackendService<CaseDocument, Query> CaseDocumentBackendService => CaseDocumentService;
}