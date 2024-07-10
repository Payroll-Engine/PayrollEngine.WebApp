using PayrollEngine.Client.Model;
using Microsoft.AspNetCore.Components;
using PayrollEngine.WebApp.Presentation;
using PayrollEngine.WebApp.Presentation.BackendService;
using PayrollEngine.WebApp.Server.Shared;
using Microsoft.Extensions.Configuration;

namespace PayrollEngine.WebApp.Server.Pages;

public partial class EmployeeCases
{
    [Inject]
    private EmployeeCaseChangeValueBackendService CaseValueService { get; set; }
    [Inject]
    private EmployeeCaseDocumentBackendService CaseDocumentService { get; set; }
    [Inject]
    private IConfiguration Configuration { get; set; }

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

    private MarkupString GetInvalidUserText()
    {
        var adminEmail = Configuration.GetConfiguration<AppConfiguration>().AdminEmail;
        if (string.IsNullOrEmpty(adminEmail))
        {
            return new($"{Localizer.App.AccessDenied}. {Localizer.App.AdminContact}.");
        }
        return new($"{Localizer.App.AccessDenied}. <a href=\"mailto:{adminEmail}\">{Localizer.App.AdminContact}</a>.");
    }
}