using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PayrollEngine.Client.Service;
using PayrollEngine.WebApp.ViewModel;

namespace PayrollEngine.WebApp.Presentation.Regulation.Factory;

public class ReportFactory : ItemFactoryBase<RegulationReport>
{
    private IReportService ReportService { get; }
    private IPayrollService PayrollService { get; }

    public ReportFactory(IReportService reportService, IPayrollService payrollService,
        Client.Model.Tenant tenant, Client.Model.Payroll payroll,
        List<Client.Model.Regulation> regulations) :
        base(tenant, payroll, regulations)
    {
        ReportService = reportService;
        PayrollService = payrollService;
    }

    protected override async Task<List<RegulationReport>> QueryItems(Client.Model.Regulation regulation) =>
        await ReportService.QueryAsync<RegulationReport>(new(Tenant.Id, regulation.Id));

    public override async Task<List<RegulationReport>> QueryPayrollItems() =>
        await PayrollService.GetReportsAsync<RegulationReport>(new(Tenant.Id, Payroll.Id));

    public async Task<bool> SaveItem(ICollection<RegulationReport> reports,
        RegulationReport report)
    {
        // regulation
        var regulation = Regulations?.FirstOrDefault();
        if (regulation == null)
        {
            return false;
        }

        // create or update
        if (report.Id == 0)
        {
            var createdCase = await ReportService.CreateAsync(new(Tenant.Id, regulation.Id), report);
            SetCreatedData(createdCase, report);
        }
        else
        {
            await ReportService.UpdateAsync(new(Tenant.Id, regulation.Id), report);
            SetUpdatedData(report);
        }

        // collection update
        return AddCollectionObject(reports, report);
    }

    public async Task<RegulationReport> DeleteItem(ICollection<RegulationReport> reports,
        RegulationReport report)
    {
        // regulation
        var regulation = Regulations?.FirstOrDefault();
        if (report.Id <= 0 || regulation == null)
        {
            return null;
        }

        // delete
        await ReportService.DeleteAsync(new(Tenant.Id, regulation.Id), report.Id);
        return DeleteCollectionObject(reports, report);
    }
}