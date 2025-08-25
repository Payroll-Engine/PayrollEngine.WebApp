using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PayrollEngine.Client.Service;
using PayrollEngine.WebApp.Presentation.Regulation.Factory;
using PayrollEngine.WebApp.ViewModel;
using Payroll = PayrollEngine.Client.Model.Payroll;
using Tenant = PayrollEngine.Client.Model.Tenant;

namespace PayrollEngine.WebApp.Presentation.Regulation.Component;

internal sealed class LookupBrowser : ItemBrowserBase
{
    internal LookupBrowser(Tenant tenant, Payroll payroll, List<Client.Model.Regulation> regulations,
        IPayrollService payrollService, ILookupService lookupService) :
        base(tenant, payroll, regulations, payrollService)
    {
        LookupService = lookupService ?? throw new ArgumentNullException(nameof(lookupService));
    }

    private ItemCollection<RegulationLookup> lookups;
    private ILookupService LookupService { get; }
    private LookupFactory lookupFactory;
    internal LookupFactory LookupFactory => lookupFactory ??=
        new(Tenant, Payroll, Regulations, PayrollService, LookupService);

    internal ItemCollection<RegulationLookup> Lookups => 
        lookups ??= LoadLookups();
    internal override async Task<bool> SaveAsync(IRegulationItem item) =>
        await LookupFactory.SaveItem(lookups, item as RegulationLookup);
    internal override async Task<IRegulationItem> DeleteAsync(IRegulationItem item) =>
        await LookupFactory.DeleteItem(lookups, item as RegulationLookup);

    protected override void OnContextChanged()
    {
        lookups = null;
        lookupFactory = null;
    }

    protected override void OnDispose() =>
        lookups?.Dispose();

    private ItemCollection<RegulationLookup> LoadLookups()
    {
        var items = System.Threading.Tasks.Task.Run(LookupFactory.LoadPayrollItemsAsync).Result;
        return new ItemCollection<RegulationLookup>(items);
    }
}