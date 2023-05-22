using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PayrollEngine.Client.Service;
using PayrollEngine.WebApp.ViewModel;

namespace PayrollEngine.WebApp.Presentation.Regulation.Factory;

public class ScriptFactory : ItemFactoryBase<RegulationScript>
{
    public IScriptService ScriptService { get; set; }
    public IPayrollService PayrollService { get; set; }

    public ScriptFactory(IScriptService scriptService, IPayrollService payrollService,
        Client.Model.Tenant tenant, Client.Model.Payroll payroll,
        List<Client.Model.Regulation> regulations) :
        base(tenant, payroll, regulations)
    {
        ScriptService = scriptService;
        PayrollService = payrollService;
    }

    public override async Task<List<RegulationScript>> QueryItems(Client.Model.Regulation regulation) =>
        await ScriptService.QueryAsync<RegulationScript>(new(Tenant.Id, regulation.Id));

    public override async Task<List<RegulationScript>> QueryPayrollItems() =>
        await PayrollService.GetScriptsAsync<RegulationScript>(new(Tenant.Id, Payroll.Id));

    public override async Task<bool> SaveItem(ICollection<RegulationScript> scripts, RegulationScript script)
    {
        // regulation
        var regulation = Regulations?.FirstOrDefault();
        if (regulation == null)
        {
            return false;
        }

        // create or update
        if (script.Id == 0)
        {
            var createdCase = await ScriptService.CreateAsync(new(Tenant.Id, regulation.Id), script);
            SetCreatedData(createdCase, script);
        }
        else
        {
            await ScriptService.UpdateAsync(new(Tenant.Id, regulation.Id), script);
            SetUpdatedData(script);
        }

        // collection update
        return AddCollectionObject(scripts, script);
    }

    public override async Task<RegulationScript> DeleteItem(ICollection<RegulationScript> scripts, RegulationScript script)
    {
        // regulation
        var regulation = Regulations?.FirstOrDefault();
        if (script.Id <= 0 || regulation == null)
        {
            return null;
        }

        // delete
        await ScriptService.DeleteAsync(new(Tenant.Id, regulation.Id), script.Id);
        return DeleteCollectionObject(scripts, script);
    }
}