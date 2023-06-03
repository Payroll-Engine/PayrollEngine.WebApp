using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PayrollEngine.Client;
using PayrollEngine.Client.Service;
using PayrollEngine.Client.Service.Api;
using PayrollEngine.Document;
using PayrollEngine.WebApp.Presentation;
using PayrollEngine.WebApp.Presentation.BackendService;
using PayrollEngine.WebApp.Server.Shared;

namespace PayrollEngine.WebApp.Server;

public static class ServiceRegistration
{
    public static void AddAppServices(this IServiceCollection services, IConfiguration configuration)
    {
        // http configuration
        var httpConfiguration = configuration.GetConfiguration<PayrollHttpConfiguration>();
        if (httpConfiguration == null)
        {
            throw new PayrollException("Missing http configuration");
        }

        // http connection
        var httpClient = new PayrollHttpClient(httpConfiguration);
        if (!httpClient.IsConnectionAvailable())
        {
            var message = $"Payroll Engine connection failed: {httpClient.Address}";
            Log.Critical(message);

#if DEBUG
            // debug break point
            if (Debugger.IsAttached)
            {
                // please start the backend server
                Debug.WriteLine($"!!! {message} !!!");
                Debugger.Break();
            }
#endif

            throw new PayrollException(message);
        }

        // tenants check
        var tenantCount = Task.Run(() =>
            new TenantService(httpClient).QueryCountAsync(new())).Result;
        if (tenantCount < 1)
        {
            var error = $"No tenants available in payroll service: {httpClient.Address}";
            Log.Critical(error);
            throw new PayrollException(error);
        }

        // system
        services.AddSingleton(httpClient);

        // theme
        services.AddSingleton<IThemeService>(new ThemeService());

        // tenant
        services.AddScoped<ITenantService, TenantService>();
        services.AddScoped<TenantBackendService>();

        // user
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<UserBackendService>();

        // division
        services.AddScoped<IDivisionService, DivisionService>();
        services.AddScoped<DivisionBackendService>();

        // employee
        services.AddScoped<IEmployeeService, EmployeeService>();
        services.AddScoped<EmployeeBackendService>();

        // payroll
        services.AddScoped<IPayrollService, PayrollService>();
        services.AddScoped<PayrollBackendService>();

        // regulation
        services.AddScoped<ICaseService, CaseService>();
        services.AddScoped<CaseBackendService>();
        services.AddScoped<ICaseFieldService, CaseFieldService>();
        services.AddScoped<ICaseRelationService, CaseRelationService>();
        services.AddScoped<ICollectorService, CollectorService>();
        services.AddScoped<IWageTypeService, WageTypeService>();
        services.AddScoped<WageTypeBackendService>();
        services.AddScoped<IReportSetService, ReportSetService>();
        services.AddScoped<IReportParameterService, ReportParameterService>();
        services.AddScoped<IReportTemplateService, ReportTemplateService>();
        services.AddScoped<ILookupService, LookupService>();
        services.AddScoped<ILookupValueService, LookupValueService>();
        services.AddScoped<IScriptService, ScriptService>();
        services.AddScoped<IRegulationService, RegulationService>();
        services.AddScoped<RegulationBackendService>();

        // payroll result
        services.AddScoped<IPayrollResultService, PayrollResultService>();
        services.AddScoped<IPayrollResultValueService, PayrollResultValueService>();
        services.AddScoped<PayrollResultBackendService>();

        // payroll layer
        services.AddScoped<IPayrollLayerService, PayrollLayerService>();
        services.AddScoped<PayrollLayerBackendService>();

        // case values
        services.AddScoped<IEmployeeCaseValueService, EmployeeCaseValueService>();
        services.AddScoped<ICompanyCaseValueService, CompanyCaseValueService>();
        services.AddScoped<INationalCaseValueService, NationalCaseValueService>();
        services.AddScoped<IGlobalCaseValueService, GlobalCaseValueService>();

        // case changes
        services.AddScoped<IEmployeeCaseChangeService, EmployeeCaseChangeService>();
        services.AddScoped<EmployeeCaseChangeValueBackendService>();
        services.AddScoped<ICompanyCaseChangeService, CompanyCaseChangeService>();
        services.AddScoped<CompanyCaseChangeValueBackendService>();
        services.AddScoped<INationalCaseChangeService, NationalCaseChangeService>();
        services.AddScoped<NationalCaseChangeValueBackendService>();
        services.AddScoped<IGlobalCaseChangeService, GlobalCaseChangeService>();
        services.AddScoped<GlobalCaseChangeValueBackendService>();
        services.AddScoped<IPayrollCaseChangeValueService, PayrollCaseChangeValueService>();

        // case documents
        services.AddScoped<IEmployeeCaseDocumentService, EmployeeCaseDocumentService>();
        services.AddScoped<EmployeeCaseDocumentBackendService>();
        services.AddScoped<ICompanyCaseDocumentService, CompanyCaseDocumentService>();
        services.AddScoped<CompanyCaseDocumentBackendService>();
        services.AddScoped<INationalCaseDocumentService, NationalCaseDocumentService>();
        services.AddScoped<NationalCaseDocumentBackendService>();
        services.AddScoped<IGlobalCaseDocumentService, GlobalCaseDocumentService>();
        services.AddScoped<GlobalCaseDocumentBackendService>();

        // payrun
        services.AddScoped<IPayrunService, PayrunService>();
        services.AddScoped<PayrunBackendService>();
        services.AddScoped<IPayrunJobService, PayrunJobService>();
        services.AddScoped<IPayrunParameterService, PayrunParameterService>();
        services.AddScoped<PayrunPayrunJobBackendService>();

        // report
        services.AddScoped<IReportService, ReportService>();
        services.AddScoped<IDataMerge, DataMerge>();

        // case relations
        services.AddScoped<ICaseRelationService, CaseRelationService>();

        // tasks
        services.AddScoped<ITaskService, TaskService>();
        services.AddScoped<TaskBackendService>();

        // webhooks
        services.AddScoped<IWebhookService, WebhookService>();
        services.AddScoped<WebhookBackendService>();
        services.AddScoped<IWebhookMessageService, WebhookMessageService>();
        services.AddScoped<WebhookMessageBackendService>();

        // logs
        services.AddScoped<ILogService, LogService>();
        services.AddScoped<IReportLogService, ReportLogService>();
        services.AddScoped<LogBackendService>();

        // shared regulations
        services.AddScoped<IRegulationShareService, RegulationShareService>();
        services.AddScoped<RegulationShareBackendService>();

        // session
        services.AddSingleton(new UserSession(
            new TenantService(httpClient),
            new DivisionService(httpClient),
            new PayrollService(httpClient),
            new EmployeeService(httpClient),
            new UserService(httpClient)));
        services.AddScoped<UserSessionBootstrap>();
        services.AddScoped<UserSessionSettings>();

        // user
        services.AddScoped<IUserNotificationService, UserNotificationService>();
        services.AddScoped<IUserPasswordService, UserPasswordService>();
    }
}