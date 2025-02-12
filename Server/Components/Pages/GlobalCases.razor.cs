using Microsoft.AspNetCore.Components;
using PayrollEngine.Client.Model;
using PayrollEngine.WebApp.Presentation;
using PayrollEngine.WebApp.Presentation.BackendService;
using PayrollEngine.WebApp.Server.Components.Shared;

namespace PayrollEngine.WebApp.Server.Components.Pages;

public partial class GlobalCases() : CasesPageBase(WorkingItems.TenantChange |
    WorkingItems.PayrollChange) 
{
    [Inject]
    private GlobalCaseChangeValueBackendService CaseValueService { get; set; }
    [Inject]
    private GlobalCaseDocumentBackendService CaseDocumentService { get; set; }

    // mandatory
    protected override CaseType CaseType => CaseType.Global;
    protected override string NewCasePageName => PageUrls.GlobalCase;
    protected override string PageTitle => Localizer.Case.GlobalCases;
    protected override string GridId => GetTenantGridId(GridIdentifiers.GlobalCaseChangeValues);

    protected override IBackendService<ViewModel.CaseChangeCaseValue, PayrollCaseChangeQuery> CaseValueBackendService => CaseValueService;
    protected override IBackendService<CaseDocument, Query> CaseDocumentBackendService => CaseDocumentService;
}