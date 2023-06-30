using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PayrollEngine.Client.Service;
using PayrollEngine.WebApp.ViewModel;

namespace PayrollEngine.WebApp.Presentation.Regulation.Factory;

public class CollectorFactory : ItemFactoryBase<RegulationCollector>
{
    private ICollectorService CollectorService { get; }
    private IPayrollService PayrollService { get; }

    public CollectorFactory(ICollectorService collectorService, IPayrollService payrollService,
        Client.Model.Tenant tenant, Client.Model.Payroll payroll,
        List<Client.Model.Regulation> regulations) :
        base(tenant, payroll, regulations)
    {
        CollectorService = collectorService;
        PayrollService = payrollService;
    }

    protected override async Task<List<RegulationCollector>> QueryItems(Client.Model.Regulation regulation) =>
        await CollectorService.QueryAsync<RegulationCollector>(new(Tenant.Id, regulation.Id));

    public override async Task<List<RegulationCollector>> QueryPayrollItems() =>
        await PayrollService.GetCollectorsAsync<RegulationCollector>(new(Tenant.Id, Payroll.Id));

    public async Task<bool> SaveItem(ICollection<RegulationCollector> collectors, RegulationCollector collector)
    {
        // regulation
        var regulation = Regulations?.FirstOrDefault();
        if (regulation == null)
        {
            return false;
        }

        // create or update
        if (collector.Id == 0)
        {
            var createdCase = await CollectorService.CreateAsync(new(Tenant.Id, regulation.Id), collector);
            SetCreatedData(createdCase, collector);
        }
        else
        {
            await CollectorService.UpdateAsync(new(Tenant.Id, regulation.Id), collector);
            SetUpdatedData(collector);
        }

        // collection update
        return AddCollectionObject(collectors, collector);
    }

    public async Task<RegulationCollector> DeleteItem(ICollection<RegulationCollector> collectors, RegulationCollector collector)
    {
        // regulation
        var regulation = Regulations?.FirstOrDefault();
        if (collector.Id <= 0 || regulation == null)
        {
            return null;
        }

        // delete
        await CollectorService.DeleteAsync(new(Tenant.Id, regulation.Id), collector.Id);
        return DeleteCollectionObject(collectors, collector);
    }
}