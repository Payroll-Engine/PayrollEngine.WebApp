using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PayrollEngine.Client.Service;
using PayrollEngine.WebApp.ViewModel;

namespace PayrollEngine.WebApp.Presentation.Regulation.Factory;

public class WageTypeFactory : ItemFactoryBase<RegulationWageType>
{
    public IWageTypeService WageTypeService { get; set; }
    public IPayrollService PayrollService { get; set; }

    public WageTypeFactory(IWageTypeService wageTypeService, IPayrollService payrollService,
        Client.Model.Tenant tenant, Client.Model.Payroll payroll,
        List<Client.Model.Regulation> regulations) :
        base(tenant, payroll, regulations)
    {
        WageTypeService = wageTypeService;
        PayrollService = payrollService;
    }

    public override async Task<List<RegulationWageType>> QueryItems(Client.Model.Regulation regulation) =>
        await WageTypeService.QueryAsync<RegulationWageType>(new(Tenant.Id, regulation.Id));

    public override async Task<List<RegulationWageType>> QueryPayrollItems() =>
        await PayrollService.GetWageTypesAsync<RegulationWageType>(new(Tenant.Id, Payroll.Id));

    public override async Task<bool> SaveItem(ICollection<RegulationWageType> wageTypes, RegulationWageType wageType)
    {
        // regulation
        var regulation = Regulations?.FirstOrDefault();
        if (regulation == null)
        {
            return false;
        }

        // create or update
        if (wageType.Id == 0)
        {
            var createdCase = await WageTypeService.CreateAsync(new(Tenant.Id, regulation.Id), wageType);
            SetCreatedData(createdCase, wageType);
        }
        else
        {
            await WageTypeService.UpdateAsync(new(Tenant.Id, regulation.Id), wageType);
            SetUpdatedData(wageType);
        }

        // collection update
        return AddCollectionObject(wageTypes, wageType);
    }

    public override async Task<RegulationWageType> DeleteItem(ICollection<RegulationWageType> wageTypes,
        RegulationWageType wageType)
    {
        // regulation
        var regulation = Regulations?.FirstOrDefault();
        if (wageType.Id <= 0 || regulation == null)
        {
            return null;
        }

        // delete
        await WageTypeService.DeleteAsync(new(Tenant.Id, regulation.Id), wageType.Id);
        return DeleteCollectionObject(wageTypes, wageType);
    }
}