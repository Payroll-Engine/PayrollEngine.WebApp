using Microsoft.AspNetCore.Components;
using PayrollEngine.Client.Model;
using PayrollEngine.WebApp.Presentation;
using PayrollEngine.WebApp.Presentation.BackendService;
using PayrollEngine.WebApp.Server.Shared;

namespace PayrollEngine.WebApp.Server.Pages;

public partial class NationalCases
{
    [Inject]
    private NationalCaseChangeValueBackendService CaseValueService { get; set; }
    [Inject]
    private NationalCaseDocumentBackendService CaseDocumentService { get; set; }

    public NationalCases() :
        base(WorkingItems.TenantChange | WorkingItems.PayrollChange)
    {
    }

    // mandatory
    protected override CaseType CaseType => CaseType.National;
    protected override string NewCasePageName => PageUrls.NationalCase;
    protected override string PageTitle => Localizer.Case.NationalCases;
    protected override string GridId => GetTenantGridId(GridIdentifiers.NationalCaseChangeValues);

    protected override IBackendService<ViewModel.CaseChangeCaseValue, PayrollCaseChangeQuery> CaseValueBackendService => CaseValueService;
    protected override IBackendService<CaseDocument, Query> CaseDocumentBackendService => CaseDocumentService;
}
