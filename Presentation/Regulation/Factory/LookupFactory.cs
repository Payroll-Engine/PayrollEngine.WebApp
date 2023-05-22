using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PayrollEngine.Client.Service;
using PayrollEngine.WebApp.ViewModel;

namespace PayrollEngine.WebApp.Presentation.Regulation.Factory;

public class LookupFactory : ItemFactoryBase<RegulationLookup>
{
    public ILookupService LookupService { get; set; }
    public IPayrollService PayrollService { get; set; }

    public LookupFactory(ILookupService lookupService, IPayrollService payrollService,
        Client.Model.Tenant tenant, Client.Model.Payroll payroll,
        List<Client.Model.Regulation> regulations) :
        base(tenant, payroll, regulations)
    {
        LookupService = lookupService;
        PayrollService = payrollService;
    }

    public override async Task<List<RegulationLookup>> QueryItems(Client.Model.Regulation regulation) =>
        await LookupService.QueryAsync<RegulationLookup>(new(Tenant.Id, regulation.Id));

    public override async Task<List<RegulationLookup>> QueryPayrollItems() =>
        await PayrollService.GetLookupsAsync<RegulationLookup>(new(Tenant.Id, Payroll.Id));

    public override async Task<bool> SaveItem(ICollection<RegulationLookup> lookups, RegulationLookup lookup)
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

    public override async Task<RegulationLookup> DeleteItem(ICollection<RegulationLookup> lookups,
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