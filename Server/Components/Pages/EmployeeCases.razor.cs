using Microsoft.AspNetCore.Components;
using PayrollEngine.Client.Model;
using PayrollEngine.WebApp.Presentation;
using PayrollEngine.WebApp.Server.Components.Shared;
using PayrollEngine.WebApp.Presentation.BackendService;

namespace PayrollEngine.WebApp.Server.Components.Pages;

public partial class EmployeeCases
{
    [Inject]
    private EmployeeCaseChangeValueBackendService CaseValueService { get; set; }
    [Inject]
    private EmployeeCaseDocumentBackendService CaseDocumentService { get; set; }

    // ReSharper disable once ConvertToPrimaryConstructor
    public EmployeeCases() :
        // user working items handled dynamic (property overload)
        base(WorkingItems.TenantChange | WorkingItems.PayrollChange)
    {
    }

    /// <summary>
    /// Employee working items
    /// </summary>
    protected override WorkingItems WorkingItems =>
        Session.User.UserType == UserType.Employee ?
            base.WorkingItems | WorkingItems.EmployeeView :
            base.WorkingItems | WorkingItems.EmployeeChange;

    protected override CaseType CaseType => CaseType.Employee;
    protected override string NewCasePageName => PageUrls.EmployeeCase;
    protected override string PageTitle => Localizer.Case.EmployeeCases;
    protected override string GridId => GetTenantGridId(GridIdentifiers.EmployeeCaseChangeValues);

    protected override IBackendService<ViewModel.CaseChangeCaseValue, PayrollCaseChangeQuery> CaseValueBackendService => CaseValueService;
    protected override IBackendService<CaseDocument, Query> CaseDocumentBackendService => CaseDocumentService;

    private bool IsValidUser()
    {
        if (User.UserType != UserType.Employee)
        {
            return true;
        }

        // self-service employee match with user
        return Employee != null &&
               string.Equals(Employee.Identifier, User.Identifier);
    }
}