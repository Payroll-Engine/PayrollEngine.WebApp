using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PayrollEngine.Client.Service;
using PayrollEngine.WebApp.ViewModel;

namespace PayrollEngine.WebApp.Presentation.Regulation.Factory;

public class LookupFactory(Client.Model.Tenant tenant, Client.Model.Payroll payroll,
        List<Client.Model.Regulation> regulations, IPayrollService payrollService,
        ILookupService lookupService)
    : ItemFactoryBase<RegulationLookup>(tenant, payroll, regulations)
{
    private ILookupService LookupService { get; } = lookupService;
    private IPayrollService PayrollService { get; } = payrollService;

    protected override async Task<List<RegulationLookup>> QueryItems(Client.Model.Regulation regulation) =>
        await LookupService.QueryAsync<RegulationLookup>(new(Tenant.Id, regulation.Id));

    public override async Task<List<RegulationLookup>> QueryPayrollItems() =>
        await PayrollService.GetLookupsAsync<RegulationLookup>(new(Tenant.Id, Payroll.Id));

    public async Task<bool> SaveItem(ICollection<RegulationLookup> lookups, RegulationLookup lookup)
    {
        // regulation
        var regulation = Regulations?.FirstOrDefault();
        if (regulation == null)
        {
            return false;
        }

        // create or update
        if (lookup.Id == 0)
        {
            var createdCase = await LookupService.CreateAsync(new(Tenant.Id, regulation.Id), lookup);
            SetCreatedData(createdCase, lookup);
        }
        else
        {
            await LookupService.UpdateAsync(new(Tenant.Id, regulation.Id), lookup);
            SetUpdatedData(lookup);
        }

        // collection update
        return AddCollectionObject(lookups, lookup);
    }

    public async Task<RegulationLookup> DeleteItem(ICollection<RegulationLookup> lookups,
        RegulationLookup lookup)
    {
        // regulation
        var regulation = Regulations?.FirstOrDefault();
        if (lookup.Id <= 0 || regulation == null)
        {
            return lookup;
        }

        // delete
        await LookupService.DeleteAsync(new(Tenant.Id, regulation.Id), lookup.Id);
        return DeleteCollectionObject(lookups, lookup);
    }
}