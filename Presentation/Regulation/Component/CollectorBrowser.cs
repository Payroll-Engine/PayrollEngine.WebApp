using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PayrollEngine.Client.Service;
using PayrollEngine.WebApp.Presentation.Regulation.Factory;
using PayrollEngine.WebApp.ViewModel;
using Payroll = PayrollEngine.Client.Model.Payroll;
using Tenant = PayrollEngine.Client.Model.Tenant;

namespace PayrollEngine.WebApp.Presentation.Regulation.Component;

internal sealed class CollectorBrowser : ItemBrowserBase
{
    internal CollectorBrowser(Tenant tenant, Payroll payroll, List<Client.Model.Regulation> regulations,
        IPayrollService payrollService, ICollectorService collectorService) :
        base(tenant, payroll, regulations, payrollService)
    {
        CollectorService = collectorService ?? throw new ArgumentNullException(nameof(collectorService));
    }

    private ItemCollection<RegulationCollector> collectors;
    private ICollectorService CollectorService { get; }
    private CollectorFactory collectorFactory;
    private CollectorFactory CollectorFactory => collectorFactory ??=
        new(Tenant, Payroll, Regulations, PayrollService, CollectorService);

    internal ItemCollection<RegulationCollector> Collectors =>
        collectors ??= LoadCollectors();
    internal override async Task<bool> SaveAsync(IRegulationItem item) =>
        await CollectorFactory.SaveItem(collectors, item as RegulationCollector);
    internal override async Task<IRegulationItem> DeleteAsync(IRegulationItem item) =>
        await CollectorFactory.DeleteItem(collectors, item as RegulationCollector);

    protected override void OnContextChanged()
    {
        collectors = null;
        collectorFactory = null;
    }

    protected override void OnDispose() =>
        collectors?.Dispose();

    private ItemCollection<RegulationCollector> LoadCollectors()
    {
        var items = System.Threading.Tasks.Task.Run(CollectorFactory.LoadPayrollItemsAsync).Result;
        return new ItemCollection<RegulationCollector>(items);
    }
}