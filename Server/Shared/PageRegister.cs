#define SYSTEM_PAGES
using System;
using System.Collections.Generic;
using System.Linq;
using PayrollEngine.WebApp.Presentation;
using PayrollEngine.WebApp.Shared;

namespace PayrollEngine.WebApp.Server.Shared;

public class PageRegister(Localizer localizer)
{
    private Localizer Localizer { get; } = localizer ?? throw new ArgumentNullException(nameof(localizer));

    public List<PageGroupInfo> PageGroups => new()
        {
            new(Localizer.Payrun.Payrun, true),
            new(Localizer.Payroll.Payroll, true),
            new(Localizer.Shared.FeaturesAdmin),
            new(Localizer.Shared.FeaturesSystem)
        };

    // pages
    public List<PageInfo> Pages => GetPages();

    private List<PageInfo> GetPages()
    {
        // groups
        var groups = PageGroups;
        var payrunGroup = groups.First(x => string.Equals(x.GroupName, Localizer.Payrun.Payrun));
        var payrollGroup = groups.First(x => string.Equals(x.GroupName, Localizer.Payroll.Payroll));
        var adminGroup = groups.First(x => string.Equals(x.GroupName, Localizer.Shared.FeaturesAdmin));

#if SYSTEM_PAGES
        var systemGroup = groups.First(x => string.Equals(x.GroupName, Localizer.Shared.FeaturesSystem));
#endif

        return new()
        {
            // main
            new(Feature.Tasks, PageUrls.Tasks, Localizer.Task.Tasks),
            new(Feature.EmployeeCases, PageUrls.EmployeeCases, Localizer.Case.EmployeeCases),
            new(Feature.CompanyCases, PageUrls.CompanyCases, Localizer.Case.CompanyCases),
            new(Feature.NationalCases, PageUrls.NationalCases, Localizer.Case.NationalCases),
            new(Feature.GlobalCases, PageUrls.GlobalCases, Localizer.Case.GlobalCases),
            new(Feature.Reports, PageUrls.Reports, Localizer.Report.Reports),

            // payrun
            new(Feature.PayrunResults, PageUrls.PayrunResults, Localizer.PayrunResult.PayrunResults, payrunGroup),
            new(Feature.PayrunJobs, PageUrls.PayrunJobs, Localizer.PayrunJob.PayrunJobs, payrunGroup),
            new(Feature.Payruns, PageUrls.Payruns, Localizer.Payrun.Payruns, payrunGroup),

            // payroll
            new(Feature.SharedRegulations, PageUrls.SharedRegulations, Localizer.RegulationShare.RegulationShares, payrollGroup),
            new(Feature.Payrolls, PageUrls.Payrolls, Localizer.Payroll.Payrolls, payrollGroup),
            new(Feature.PayrollLayers, PageUrls.PayrollLayers, Localizer.PayrollLayer.PayrollLayers,  payrollGroup),
            new(Feature.Regulations, PageUrls.Regulations, Localizer.Regulation.Regulations,  payrollGroup),
            new(Feature.Regulation, PageUrls.Regulation, Localizer.Regulation.Regulation,  payrollGroup),

            // administration
            new(Feature.Employees, PageUrls.Employees, Localizer.Employee.Employees, adminGroup),
            new(Feature.Users, PageUrls.Users, Localizer.User.Users, adminGroup),
            new(Feature.Logs, PageUrls.Logs, Localizer.Log.Logs, adminGroup),
            new(Feature.Webhooks, PageUrls.Webhooks, Localizer.Webhook.Webhooks, adminGroup),
            new(Feature.Calendars, PageUrls.Calendars, Localizer.Calendar.Calendars, adminGroup),
            new(Feature.Divisions, PageUrls.Divisions, Localizer.Division.Divisions, adminGroup),
            new(Feature.Tenants, PageUrls.Tenants, Localizer.Tenant.Tenants, adminGroup)

            // system
#if SYSTEM_PAGES
            , new(Feature.UserStorage, PageUrls.UserStorage, Localizer.Storage.Storage, systemGroup)
#endif
        };
    }
}