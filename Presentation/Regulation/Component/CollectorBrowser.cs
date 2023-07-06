using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PayrollEngine.Client.Service;
using PayrollEngine.WebApp.Presentation.Regulation.Factory;
using PayrollEngine.WebApp.ViewModel;
using Payroll = PayrollEngine.Client.Model.Payroll;
using Task = System.Threading.Tasks.Task;
using Tenant = PayrollEngine.Client.Model.Tenant;

namespace PayrollEngine.WebApp.Presentation.Regulation.Component
{
    internal class CollectorBrowser : ItemBrowserBase
    {
        internal CollectorBrowser(Tenant tenant, Payroll payroll, List<Client.Model.Regulation> regulations,
            IPayrollService payrollService, ICollectorService collectorService) :
            base(tenant, payroll, regulations, payrollService)
        {
            CollectorService = collectorService ?? throw new ArgumentNullException(nameof(collectorService));
        }

        private ItemCollection<RegulationCollector> collectors;
        private CollectorFactory collectorFactory;
        private ICollectorService CollectorService { get; }

        internal override void Reset()
        {
            collectors = null;
            collectorFactory = null;
        }

        internal ItemCollection<RegulationCollector> Collectors => collectors ??= LoadCollectors();
        private CollectorFactory CollectorFactory => collectorFactory ??=
            new(Tenant, Payroll, Regulations, PayrollService, CollectorService);
        internal override async Task<bool> SaveAsync(IRegulationItem item) =>
            await CollectorFactory.SaveItem(collectors, item as RegulationCollector);

        internal override async Task<IRegulationItem> DeleteAsync(IRegulationItem item) =>
            await CollectorFactory.DeleteItem(collectors, item as RegulationCollector);

        protected override void OnDispose() =>
            collectors?.Dispose();

        private ItemCollection<RegulationCollector> LoadCollectors() =>
            new(Task.Run(CollectorFactory.LoadPayrollItems).Result);
    }
}
