using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PayrollEngine.Client.Service;
using PayrollEngine.WebApp.Presentation.Regulation.Factory;
using PayrollEngine.WebApp.ViewModel;
using Payroll = PayrollEngine.Client.Model.Payroll;
using Task = System.Threading.Tasks.Task;
using Tenant = PayrollEngine.Client.Model.Tenant;

namespace PayrollEngine.WebApp.Presentation.Regulation.Component
{
    internal class ReportTemplateBrowser : ItemBrowserBase
    {
        internal ReportTemplateBrowser(Tenant tenant, Payroll payroll, List<Client.Model.Regulation> regulations,
            IPayrollService payrollService, IReportService reportService, IReportTemplateService reportTemplateService) :
            base(tenant, payroll, regulations, payrollService)
        {
            ReportService = reportService ?? throw new ArgumentNullException(nameof(reportService));
            ReportTemplateService = reportTemplateService ?? throw new ArgumentNullException(nameof(reportTemplateService));
        }

        private ItemCollection<RegulationReportTemplate> reportTemplates;
        private ReportTemplateFactory reportTemplateFactory;
        private IReportService ReportService { get; }
        private IReportTemplateService ReportTemplateService { get; }

        internal override void Reset()
        {
            reportTemplates = null;
            reportTemplateFactory = null;
        }

        internal ItemCollection<RegulationReportTemplate> ReportTemplates => reportTemplates ??= LoadReportTemplates();
        private ReportTemplateFactory ReportTemplateFactory => reportTemplateFactory ??=
            new(Tenant, Payroll, Regulations, PayrollService, ReportService, ReportTemplateService);

        internal override async Task<bool> SaveAsync(IRegulationItem item) =>
            await ReportTemplateFactory.SaveItem(reportTemplates, item as RegulationReportTemplate);

        internal override async Task<IRegulationItem> DeleteAsync(IRegulationItem item) =>
            await ReportTemplateFactory.DeleteItem(reportTemplates, item as RegulationReportTemplate);

        protected override void OnDispose() =>
            reportTemplates?.Dispose();

        private ItemCollection<RegulationReportTemplate> LoadReportTemplates() =>
            new(Task.Run(ReportTemplateFactory.LoadPayrollItems).Result);
    }
}
