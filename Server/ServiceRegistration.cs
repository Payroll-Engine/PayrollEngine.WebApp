#if DEBUG
#define DEBUG_CONNECTION_BREAK
#endif
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor;
using PayrollEngine.Client;
using PayrollEngine.Document;
using PayrollEngine.WebApp.Shared;
using PayrollEngine.Client.Service;
using PayrollEngine.Client.Service.Api;
using PayrollEngine.WebApp.Presentation;
using PayrollEngine.WebApp.Server.Components.Shared;
using PayrollEngine.WebApp.Presentation.BackendService;

namespace PayrollEngine.WebApp.Server;

public static class ServiceRegistration
{
    public static async Task AddAppServicesAsync(this IServiceCollection services, IConfiguration configuration)
    {
        // http client configuration
        var httpConfiguration = await configuration.GetHttpConfigurationAsync();
        if (httpConfiguration == null)
        {
            throw new PayrollException("Missing Payroll HTTP configuration.");
        }
        if (!httpConfiguration.Valid())
        {
            throw new PayrollException("Invalid Payroll HTTP configuration.");
        }

        // http services
        services.AddSingleton(httpConfiguration);

        // http client
        var httpClient = await PayrollHttpClientFactory.CreatePayrollHttpClientAsync(configuration);

        // http connection
        var available = await httpClient.IsConnectionAvailableAsync(TenantApiEndpoints.TenantsUrl());
        if (!available)
        {
            var message = $"Payroll Engine backend connection failed: {httpClient.Address}.";
            Log.Critical(message);

#if DEBUG_CONNECTION_BREAK
            // debug break point
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // please start the payroll engine backend server
                System.Diagnostics.Debug.WriteLine($"!!! {message} !!!");
                System.Diagnostics.Debugger.Break();
            }
#endif

            throw new PayrollException(message);
        }

        // localizations
        services.AddLocalization(o => { o.ResourcesPath = "Resources"; });
        services.Configure<RequestLocalizationOptions>(options =>
        {
            var supportedCultures = SharedCultures.GetCultures();
            options.DefaultRequestCulture = new RequestCulture(SharedCultures.DefaultCulture);
            options.SupportedCultures = supportedCultures;
            options.SupportedUICultures = supportedCultures;
        });

        // tenants check
        var tenantCount = await new TenantService(httpClient).QueryCountAsync(new());
        if (tenantCount < 1)
        {
            var error = $"No tenants available in payroll service: {httpClient.Address}.";
            Log.Critical(error);
        }

        // web
        services.AddSingleton(httpClient);

        // pages
        var appConfiguration = configuration.GetConfiguration<AppConfiguration>();
        var appTitle = appConfiguration.AppTitle ?? SystemSpecification.ApplicationName;

        services.AddSingleton<IPageService>(new PageService(appTitle));

        // theme
        services.AddSingleton<IThemeService>(new ThemeService());

        // downloads
        services.AddSingleton<IDownloadService>(new DownloadService(appConfiguration.MaxDownloadSize));

        // localization
        services.AddTransient<Localizer>();
        services.AddTransient<MudLocalizer, AppMudLocalizer>();

        // tenant
        services.AddScoped<ITenantService, TenantService>();
        services.AddScoped<TenantBackendService>();

        // user
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<UserBackendService>();

        // calendar
        services.AddScoped<ICalendarService, CalendarService>();
        services.AddScoped<CalendarBackendService>();

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
        services.AddScoped<IReportService, ReportService>();
        services.AddScoped<ReportBackendService>();

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
            configuration,
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

        // forecast
        services.AddScoped<IForecastHistoryService, ForecastHistoryService>();
    }
}