using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PayrollEngine.Client.Service;
using PayrollEngine.WebApp.Presentation.Regulation.Factory;
using PayrollEngine.WebApp.ViewModel;
using Payroll = PayrollEngine.Client.Model.Payroll;
using Tenant = PayrollEngine.Client.Model.Tenant;

namespace PayrollEngine.WebApp.Presentation.Regulation.Component;

internal sealed class ReportTemplateBrowser : ItemBrowserBase
{
    internal ReportTemplateBrowser(Tenant tenant, Payroll payroll, List<Client.Model.Regulation> regulations,
        IPayrollService payrollService, IReportService reportService, IReportTemplateService reportTemplateService) :
        base(tenant, payroll, regulations, payrollService)
    {
        ReportService = reportService ?? throw new ArgumentNullException(nameof(reportService));
        ReportTemplateService = reportTemplateService ?? throw new ArgumentNullException(nameof(reportTemplateService));
    }

    private ItemCollection<RegulationReportTemplate> reportTemplates;
    private IReportService ReportService { get; }
    private IReportTemplateService ReportTemplateService { get; }
    private ReportTemplateFactory reportTemplateFactory;
    private ReportTemplateFactory ReportTemplateFactory => reportTemplateFactory ??=
        new(Tenant, Payroll, Regulations, PayrollService, ReportService, ReportTemplateService);

    internal ItemCollection<RegulationReportTemplate> ReportTemplates =>
        reportTemplates ??= LoadReportTemplates();
    internal override async Task<bool> SaveAsync(IRegulationItem item) =>
        await ReportTemplateFactory.SaveItem(reportTemplates, item as RegulationReportTemplate);
    internal override async Task<IRegulationItem> DeleteAsync(IRegulationItem item) =>
        await ReportTemplateFactory.DeleteItem(reportTemplates, item as RegulationReportTemplate);

    protected override void OnContextChanged()
    {
        reportTemplates = null;
        reportTemplateFactory = null;
    }

    protected override void OnDispose() =>
        reportTemplates?.Dispose();

    private ItemCollection<RegulationReportTemplate> LoadReportTemplates()
    {
        var items = System.Threading.Tasks.Task.Run(ReportTemplateFactory.LoadPayrollItemsAsync).Result;
        return new ItemCollection<RegulationReportTemplate>(items);
    }
}