using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PayrollEngine.Client.Service;
using PayrollEngine.WebApp.ViewModel;

namespace PayrollEngine.WebApp.Presentation.Regulation.Factory;

public class ScriptFactory(Client.Model.Tenant tenant, Client.Model.Payroll payroll,
        List<Client.Model.Regulation> regulations, IPayrollService payrollService
        , IScriptService scriptService)
    : ItemFactoryBase<RegulationScript>(tenant, payroll, regulations)
{
    private IScriptService ScriptService { get; } = scriptService;
    private IPayrollService PayrollService { get; } = payrollService;

    protected override async Task<List<RegulationScript>> QueryItems(Client.Model.Regulation regulation) =>
        await ScriptService.QueryAsync<RegulationScript>(new(Tenant.Id, regulation.Id));

    public override async Task<List<RegulationScript>> QueryPayrollItems() =>
        await PayrollService.GetScriptsAsync<RegulationScript>(new(Tenant.Id, Payroll.Id));

    public async Task<bool> SaveItem(ICollection<RegulationScript> scripts, RegulationScript script)
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

    public async Task<RegulationScript> DeleteItem(ICollection<RegulationScript> scripts, RegulationScript script)
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