using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PayrollEngine.Client.Service;
using PayrollEngine.WebApp.ViewModel;

namespace PayrollEngine.WebApp.Presentation.Regulation.Factory;

public class ReportTemplateFactory : ChildItemFactory<RegulationReport, RegulationReportTemplate>
{
    private IReportService ReportService { get; }
    private IReportTemplateService ReportTemplateService { get; }
    private IPayrollService PayrollService { get; }

    public ReportTemplateFactory(Client.Model.Tenant tenant, Client.Model.Payroll payroll,
        List<Client.Model.Regulation> regulations, IPayrollService payrollService,
        IReportService caseService, IReportTemplateService caseFieldService) :
        base(tenant, payroll, regulations)
    {
        ReportService = caseService;
        ReportTemplateService = caseFieldService;
        PayrollService = payrollService;
    }

    protected override async Task<List<RegulationReportTemplate>> QueryItems(Client.Model.Regulation regulation) =>
        await PayrollService.GetReportTemplatesAsync<RegulationReportTemplate>(new(Tenant.Id, Payroll.Id));

    public override async Task<List<RegulationReportTemplate>> QueryPayrollItems() =>
        await PayrollService.GetReportTemplatesAsync<RegulationReportTemplate>(new(Tenant.Id, Payroll.Id));

    public async Task<bool> SaveItem(ICollection<RegulationReportTemplate> reportTemplates,
        RegulationReportTemplate reportTemplate)
    {
        // regulation
        var regulation = Regulations?.FirstOrDefault();
        if (regulation == null)
        {
            return false;
        }

        // report
        var report = reportTemplate.Parent as RegulationReport;
        if (report == null || report.Id == 0)
        {
            throw new PayrollException($"Missing report for report template {reportTemplate.Name}");
        }

        // create or update
        if (reportTemplate.Id == 0)
        {
            var createdCase = await ReportTemplateService.CreateAsync(new(Tenant.Id, regulation.Id, report.Id), reportTemplate);
            SetCreatedData(createdCase, reportTemplate);
        }
        else
        {
            await ReportTemplateService.UpdateAsync(new(Tenant.Id, regulation.Id, report.Id), reportTemplate);
            SetUpdatedData(reportTemplate);
        }

        // collection update
        return AddCollectionObject(reportTemplates, reportTemplate);
    }

    public async Task<RegulationReportTemplate> DeleteItem(ICollection<RegulationReportTemplate> reportTemplates,
        RegulationReportTemplate reportTemplate)
    {
        // regulation
        var regulation = Regulations?.FirstOrDefault();
        if (reportTemplate.Id <= 0 || regulation == null)
        {
            return null;
        }

        // report
        var report = reportTemplate.Parent as RegulationReport;
        if (report == null || report.Id == 0)
        {
            return null;
        }

        // delete
        await ReportTemplateService.DeleteAsync(new(Tenant.Id, regulation.Id, report.Id), reportTemplate.Id);
        return DeleteCollectionObject(reportTemplates, reportTemplate);
    }

    protected override async Task<List<RegulationReport>> QueryParentObjects(int tenantId, int regulationId) =>
        await ReportService.QueryAsync<RegulationReport>(new(tenantId, regulationId));

    protected override async Task<List<RegulationReportTemplate>> QueryChildObjects(int tenantId, int regulationId, RegulationReport parentObject) =>
        await ReportTemplateService.QueryAsync<RegulationReportTemplate>(new(tenantId, regulationId, parentObject.Id));

}