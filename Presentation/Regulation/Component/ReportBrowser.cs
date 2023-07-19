using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PayrollEngine.Client.Service;
using PayrollEngine.WebApp.Presentation.Regulation.Factory;
using PayrollEngine.WebApp.ViewModel;
using Payroll = PayrollEngine.Client.Model.Payroll;
using Task = System.Threading.Tasks.Task;
using Tenant = PayrollEngine.Client.Model.Tenant;

namespace PayrollEngine.WebApp.Presentation.Regulation.Component;

internal class ReportBrowser : ItemBrowserBase
{
    internal ReportBrowser(Tenant tenant, Payroll payroll, List<Client.Model.Regulation> regulations,
        IPayrollService payrollService, IReportService reportService) :
        base(tenant, payroll, regulations, payrollService)
    {
        ReportService = reportService ?? throw new ArgumentNullException(nameof(reportService));
    }

    private ItemCollection<RegulationReport> reports;
    private ReportFactory reportFactory;
    private IReportService ReportService { get; }

    internal override void Reset()
    {
        reports = null;
        reportFactory = null;
    }

    internal ItemCollection<RegulationReport> Reports => reports ??= LoadReports();
    internal ReportFactory ReportFactory => reportFactory ??=
        new(Tenant, Payroll, Regulations, PayrollService, ReportService);
    internal override async Task<bool> SaveAsync(IRegulationItem item) =>
        await ReportFactory.SaveItem(reports, item as RegulationReport);

    internal override async Task<IRegulationItem> DeleteAsync(IRegulationItem item) =>
        await ReportFactory.DeleteItem(reports, item as RegulationReport);

    protected override void OnDispose() =>
        reports?.Dispose();

    private ItemCollection<RegulationReport> LoadReports() =>
        new(Task.Run(ReportFactory.LoadPayrollItems).Result);
}