using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PayrollEngine.Client.Service;
using PayrollEngine.WebApp.Presentation.Regulation.Factory;
using PayrollEngine.WebApp.ViewModel;
using Payroll = PayrollEngine.Client.Model.Payroll;
using Tenant = PayrollEngine.Client.Model.Tenant;

namespace PayrollEngine.WebApp.Presentation.Regulation.Component;

internal class LookupValueBrowser : ItemBrowserBase
{
    internal LookupValueBrowser(Tenant tenant, Payroll payroll, List<Client.Model.Regulation> regulations,
        IPayrollService payrollService, ILookupService lookupService, ILookupValueService lookupValueService) :
        base(tenant, payroll, regulations, payrollService)
    {
        LookupService = lookupService ?? throw new ArgumentNullException(nameof(lookupService));
        LookupValueService = lookupValueService ?? throw new ArgumentNullException(nameof(lookupValueService));
    }

    private ItemCollection<RegulationLookupValue> lookupValues;
    private ILookupService LookupService { get; }
    private ILookupValueService LookupValueService { get; }
    private LookupValueFactory lookupValueFactory;
    private LookupValueFactory LookupValueFactory => lookupValueFactory ??=
        new(Tenant, Payroll, Regulations, PayrollService, LookupService, LookupValueService);

    internal ItemCollection<RegulationLookupValue> LookupValues =>
        lookupValues ??= LoadLookupValues();
    internal override async Task<bool> SaveAsync(IRegulationItem item) =>
        await LookupValueFactory.SaveItem(lookupValues, item as RegulationLookupValue);
    internal override async Task<IRegulationItem> DeleteAsync(IRegulationItem item) =>
        await LookupValueFactory.DeleteItem(lookupValues, item as RegulationLookupValue);

    protected override void OnContextChanged()
    {
        lookupValues = null;
        lookupValueFactory = null;
    }

    protected override void OnDispose() =>
        lookupValues?.Dispose();

    private ItemCollection<RegulationLookupValue> LoadLookupValues()
    {
        var items = System.Threading.Tasks.Task.Run(LookupValueFactory.LoadPayrollItemsAsync).Result;
        return new ItemCollection<RegulationLookupValue>(items);
    }
}