﻿using Microsoft.AspNetCore.Components;
using PayrollEngine.Client.Model;
using PayrollEngine.WebApp.Presentation;
using PayrollEngine.WebApp.Presentation.BackendService;
using PayrollEngine.WebApp.Server.Shared;

namespace PayrollEngine.WebApp.Server.Pages;

public partial class GlobalCases
{
    [Inject]
    private GlobalCaseChangeValueBackendService CaseValueService { get; set; }
    [Inject]
    private GlobalCaseDocumentBackendService CaseDocumentService { get; set; }

    public GlobalCases() :
        base(WorkingItems.TenantChange | WorkingItems.PayrollChange)
    {
    }

    // mandatory
    protected override CaseType CaseType => CaseType.Global;
    protected override string NewCasePageName => PageUrls.GlobalCase;
    protected override string PageTitle => Localizer.Case.GlobalCases;
    protected override string GridId => GetTenantGridId(GridIdentifiers.GlobalCaseChangeValues);

    protected override IBackendService<ViewModel.CaseChangeCaseValue, PayrollCaseChangeQuery> CaseValueBackendService => CaseValueService;
    protected override IBackendService<CaseDocument, Query> CaseDocumentBackendService => CaseDocumentService;
}