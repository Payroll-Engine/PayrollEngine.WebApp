#define SYSTEM_PAGES
using System.Collections.Generic;
using System.Linq;
using PayrollEngine.WebApp.Presentation;

namespace PayrollEngine.WebApp.Server.Shared;

public static class PageRegister
{

    public static List<PageGroupInfo> PageGroups => new()
        {
            new("Payrun", true),
            new("Payroll", true),
            new("Administration"),
            new("System")
        };

    // pages
    public static List<PageInfo> Pages => GetPages();

    private static List<PageInfo> GetPages()
    {
        // groups
        var groups = PageGroups;
        var payrunGroup = groups.First(x => string.Equals(x.GroupName, "Payrun"));
        var payrollGroup = groups.First(x => string.Equals(x.GroupName, "Payroll"));
        var adminGroup = groups.First(x => string.Equals(x.GroupName, "Administration"));

#if SYSTEM_PAGES
        var systemGroup = groups.First(x => string.Equals(x.GroupName, "System"));
#endif

        return new()
        {
            // Main
            new(Feature.Tasks, PageUrls.Tasks, "Tasks"),
            new(Feature.EmployeeCases, PageUrls.EmployeeCases, "Employee Cases"),
            new(Feature.CompanyCases, PageUrls.CompanyCases, "Company Cases"),
            new(Feature.NationalCases, PageUrls.NationalCases, "National Cases"),
            new(Feature.GlobalCases, PageUrls.GlobalCases, "Global Cases"),
            new(Feature.Reports, PageUrls.Reports, "Reports"),
            // Payrun
            new(Feature.PayrunResults, PageUrls.PayrunResults, "Payrun Results", payrunGroup),
            new(Feature.PayrunJobs, PageUrls.PayrunJobs, "Payrun Jobs", payrunGroup),
            new(Feature.Payruns, PageUrls.Payruns, "Payruns", payrunGroup),
            // Payroll
            new(Feature.SharedRegulations, PageUrls.SharedRegulations, "Shared Regulations", payrollGroup),
            new(Feature.Payrolls, PageUrls.Payrolls, "Payrolls", payrollGroup),
            new(Feature.PayrollLayers, PageUrls.PayrollLayers, "Payroll Layers",  payrollGroup),
            new(Feature.Regulations, PageUrls.Regulations, "Regulations",  payrollGroup),
            new(Feature.Regulation, PageUrls.Regulation, "Regulation",  payrollGroup),
            // Administration
            new(Feature.Tenants, PageUrls.Tenants, "Tenants", adminGroup),
            new(Feature.Users, PageUrls.Users, "Users", adminGroup),
            new(Feature.Calendars, PageUrls.Calendars, "Calendars", adminGroup),
            new(Feature.Divisions, PageUrls.Divisions, "Divisions", adminGroup),
            new(Feature.Employees, PageUrls.Employees, "Employees", adminGroup),
            new(Feature.Webhooks, PageUrls.Webhooks, "Webhooks", adminGroup),
            new(Feature.Logs, PageUrls.Logs, "Logs", adminGroup)
            // System
#if SYSTEM_PAGES
            , new(Feature.UserStorage, PageUrls.UserStorage, "User Storage", systemGroup)
#endif
        };
    }
}