using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PayrollEngine.Client.Service;
using PayrollEngine.WebApp.Presentation.Regulation.Factory;
using PayrollEngine.WebApp.ViewModel;
using Payroll = PayrollEngine.Client.Model.Payroll;
using Tenant = PayrollEngine.Client.Model.Tenant;

namespace PayrollEngine.WebApp.Presentation.Regulation.Component;

internal class ReportParameterBrowser : ItemBrowserBase
{
    internal ReportParameterBrowser(Tenant tenant, Payroll payroll, List<Client.Model.Regulation> regulations,
        IPayrollService payrollService, IReportService reportService, IReportParameterService reportParameterService) :
        base(tenant, payroll, regulations, payrollService)
    {
        ReportService = reportService ?? throw new ArgumentNullException(nameof(reportService));
        ReportParameterService = reportParameterService ?? throw new ArgumentNullException(nameof(reportParameterService));
    }

    private ItemCollection<RegulationReportParameter> reportParameters;
    private IReportService ReportService { get; }
    private IReportParameterService ReportParameterService { get; }
    private ReportParameterFactory reportParameterFactory;
    private ReportParameterFactory ReportParameterFactory => reportParameterFactory ??=
        new(Tenant, Payroll, Regulations, PayrollService, ReportService, ReportParameterService);

    internal ItemCollection<RegulationReportParameter> ReportParameters => 
        reportParameters ??= LoadReportParameters();
    internal override async Task<bool> SaveAsync(IRegulationItem item) =>
        await ReportParameterFactory.SaveItem(reportParameters, item as RegulationReportParameter);
    internal override async Task<IRegulationItem> DeleteAsync(IRegulationItem item) =>
        await ReportParameterFactory.DeleteItem(reportParameters, item as RegulationReportParameter);

    protected override void OnContextChanged()
    {
        reportParameters = null;
        reportParameterFactory = null;
    }

    protected override void OnDispose() =>
        reportParameters?.Dispose();

    private ItemCollection<RegulationReportParameter> LoadReportParameters()
    {
        var items = System.Threading.Tasks.Task.Run(ReportParameterFactory.LoadPayrollItemsAsync).Result;
        return new ItemCollection<RegulationReportParameter>(items);
    }
}