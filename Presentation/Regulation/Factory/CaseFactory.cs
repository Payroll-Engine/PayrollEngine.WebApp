using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PayrollEngine.Client.Service;
using PayrollEngine.WebApp.ViewModel;

namespace PayrollEngine.WebApp.Presentation.Regulation.Factory;

public class CaseFactory : ItemFactoryBase<RegulationCase>
{
    private ICaseService CaseService { get; }
    private IPayrollService PayrollService { get; }

    public CaseFactory(Client.Model.Tenant tenant, Client.Model.Payroll payroll,
        List<Client.Model.Regulation> regulations, IPayrollService payrollService, ICaseService caseService) :
        base(tenant, payroll, regulations)
    {
        CaseService = caseService;
        PayrollService = payrollService;
    }

    protected override async Task<List<RegulationCase>> QueryItems(Client.Model.Regulation regulation) =>
        await CaseService.QueryAsync<RegulationCase>(new(Tenant.Id, regulation.Id));

    public override async Task<List<RegulationCase>> QueryPayrollItems() =>
        await PayrollService.GetCasesAsync<RegulationCase>(new(Tenant.Id, Payroll.Id));

    public async Task<bool> SaveItem(ICollection<RegulationCase> cases, RegulationCase @case)
    {
        // regulation
        var regulation = Regulations?.FirstOrDefault();
        if (regulation == null)
        {
            return false;
        }

        // create or update
        if (@case.Id == 0)
        {
            var createdCase = await CaseService.CreateAsync(new(Tenant.Id, regulation.Id), @case);
            SetCreatedData(createdCase, @case);
        }
        else
        {
            await CaseService.UpdateAsync(new(Tenant.Id, regulation.Id), @case);
            SetUpdatedData(@case);
        }

        // collection update
        return AddCollectionObject(cases, @case);
    }

    public async Task<RegulationCase> DeleteItem(ICollection<RegulationCase> cases, RegulationCase @case)
    {
        var regulation = Regulations?.FirstOrDefault();
        if (@case.Id <= 0 || regulation == null)
        {
            return null;
        }

        // delete
        await CaseService.DeleteAsync(new(Tenant.Id, regulation.Id), @case.Id);
        return DeleteCollectionObject(cases, @case);
    }
}