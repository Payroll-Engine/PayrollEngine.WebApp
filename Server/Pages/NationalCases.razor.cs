using Microsoft.AspNetCore.Components;
using PayrollEngine.Client.Model;
using PayrollEngine.Client.Service;
using PayrollEngine.WebApp.Presentation;
using PayrollEngine.WebApp.Presentation.BackendService;
using PayrollEngine.WebApp.Server.Shared;

namespace PayrollEngine.WebApp.Server.Pages;

public partial class NationalCases
{
    [Inject]
    protected INationalCaseValueService NationalCaseValueService { get; set; }
    [Inject]
    protected INationalCaseChangeService NationalCaseChangeService { get; set; }
    [Inject]
    protected NationalCaseChangeValueBackendService CaseValueService { get; set; }
    [Inject]
    protected NationalCaseDocumentBackendService CaseDocumentService { get; set; }

    public NationalCases() :
        base(WorkingItems.TenantChange | WorkingItems.PayrollChange)
    {
    }

    // mandatory
    protected override CaseType CaseType => CaseType.National;
    protected override string NewCasePageName => PageUrls.NationalCase;
    protected override string PageTitle => "National Cases";
    protected override string GridId => GetTenantGridId(GridIdentifiers.NationalCaseChangeValues);

    protected override IBackendService<ViewModel.CaseChangeCaseValue, PayrollCaseChangeQuery> CaseValueBackendService => CaseValueService;
    protected override IBackendService<CaseDocument, Query> CaseDocumentBackendService => CaseDocumentService;
}
