﻿using Microsoft.AspNetCore.Components;
using PayrollEngine.Client.Model;
using PayrollEngine.WebApp.Presentation;
using PayrollEngine.WebApp.Presentation.BackendService;
using PayrollEngine.WebApp.Server.Components.Shared;

namespace PayrollEngine.WebApp.Server.Components.Pages;

public partial class NationalCases() : CasesPageBase(WorkingItems.TenantChange |
    WorkingItems.PayrollChange) 
{
    [Inject]
    private NationalCaseChangeValueBackendService CaseValueService { get; set; }
    [Inject]
    private NationalCaseDocumentBackendService CaseDocumentService { get; set; }

    // mandatory
    protected override CaseType CaseType => CaseType.National;
    protected override string NewCasePageName => PageUrls.NationalCase;
    protected override string PageTitle => Localizer.Case.NationalCases;
    protected override string GridId => GetTenantGridId(GridIdentifiers.NationalCaseChangeValues);

    protected override IBackendService<ViewModel.CaseChangeCaseValue, PayrollCaseChangeQuery> CaseValueBackendService => CaseValueService;
    protected override IBackendService<CaseDocument, Query> CaseDocumentBackendService => CaseDocumentService;
}
