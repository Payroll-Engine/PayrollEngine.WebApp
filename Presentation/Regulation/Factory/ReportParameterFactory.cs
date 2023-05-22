﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PayrollEngine.Client.Service;
using PayrollEngine.WebApp.ViewModel;

namespace PayrollEngine.WebApp.Presentation.Regulation.Factory;

public class ReportParameterFactory : ChildItemFactory<RegulationReport, RegulationReportParameter>
{
    public IReportService ReportService { get; set; }
    public IReportParameterService ReportParameterService { get; set; }
    public IPayrollService PayrollService { get; set; }

    public ReportParameterFactory(IReportService caseService, IReportParameterService reportParameterService,
        IPayrollService payrollService,
        Client.Model.Tenant tenant, Client.Model.Payroll payroll,
        List<Client.Model.Regulation> regulations) :
        base(tenant, payroll, regulations)
    {
        ReportService = caseService;
        ReportParameterService = reportParameterService;
        PayrollService = payrollService;
    }

    public override async Task<List<RegulationReportParameter>> QueryItems(Client.Model.Regulation regulation) =>
        await PayrollService.GetReportParametersAsync<RegulationReportParameter>(new(Tenant.Id, Payroll.Id));

    public override async Task<List<RegulationReportParameter>> QueryPayrollItems() =>
        await PayrollService.GetReportParametersAsync<RegulationReportParameter>(new(Tenant.Id, Payroll.Id));

    public override async Task<bool> SaveItem(ICollection<RegulationReportParameter> reportParameters,
        RegulationReportParameter reportParameter)
    {
        var regulation = Regulations?.FirstOrDefault();
        if (regulation == null)
        {
            return false;
        }

        // report
        var report = reportParameter.Parent as RegulationReport;
        if (report == null || report.Id == 0)
        {
            throw new PayrollException($"Missing report for report parameter {reportParameter.Name}");
        }

        // create or update
        if (reportParameter.Id == 0)
        {
            var createdCase = await ReportParameterService.CreateAsync(new(Tenant.Id, regulation.Id, report.Id), reportParameter);
            SetCreatedData(createdCase, reportParameter);
        }
        else
        {
            await ReportParameterService.UpdateAsync(new(Tenant.Id, regulation.Id, report.Id), reportParameter);
            SetUpdatedData(reportParameter);
        }

        // collection update
        return AddCollectionObject(reportParameters, reportParameter);
    }

    public override async Task<RegulationReportParameter> DeleteItem(ICollection<RegulationReportParameter> reportParameters,
        RegulationReportParameter reportParameter)
    {
        // regulation
        var regulation = Regulations?.FirstOrDefault();
        if (reportParameter.Id <= 0 || regulation == null)
        {
            return null;
        }

        // report
        var report = reportParameter.Parent as RegulationReport;
        if (report == null || report.Id == 0)
        {
            return null;
        }

        // delete
        await ReportParameterService.DeleteAsync(new(Tenant.Id, regulation.Id, report.Id), reportParameter.Id);
        return DeleteCollectionObject(reportParameters, reportParameter);
    }

    protected override async Task<List<RegulationReport>> QueryParentObjects(int tenantId, int regulationId) =>
        await ReportService.QueryAsync<RegulationReport>(new(tenantId, regulationId));

    protected override async Task<List<RegulationReportParameter>> QueryChildObjects(int tenantId, RegulationReport parentObject) =>
        await ReportParameterService.QueryAsync<RegulationReportParameter>(new(tenantId, parentObject.RegulationId, parentObject.Id));

}