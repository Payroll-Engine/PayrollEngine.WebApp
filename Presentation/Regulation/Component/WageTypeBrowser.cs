using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PayrollEngine.Client.Service;
using PayrollEngine.WebApp.Presentation.Regulation.Factory;
using PayrollEngine.WebApp.ViewModel;
using Payroll = PayrollEngine.Client.Model.Payroll;
using Tenant = PayrollEngine.Client.Model.Tenant;

namespace PayrollEngine.WebApp.Presentation.Regulation.Component;

internal class WageTypeBrowser : ItemBrowserBase
{
    internal WageTypeBrowser(Tenant tenant, Payroll payroll, List<Client.Model.Regulation> regulations,
        IPayrollService payrollService, IWageTypeService wageTypeService) :
        base(tenant, payroll, regulations, payrollService)
    {
        WageTypeService = wageTypeService ?? throw new ArgumentNullException(nameof(wageTypeService));
    }

    private ItemCollection<RegulationWageType> wageTypes;
    private IWageTypeService WageTypeService { get; }
    private WageTypeFactory wageTypeFactory;
    private WageTypeFactory WageTypeFactory => wageTypeFactory ??=
        new(Tenant, Payroll, Regulations, PayrollService, WageTypeService);

    internal ItemCollection<RegulationWageType> WageTypes =>
        wageTypes ??= LoadWageTypes();
    internal override async Task<bool> SaveAsync(IRegulationItem item) =>
        await WageTypeFactory.SaveItem(wageTypes, item as RegulationWageType);
    internal override async Task<IRegulationItem> DeleteAsync(IRegulationItem item) =>
        await WageTypeFactory.DeleteItem(wageTypes, item as RegulationWageType);

    protected override void OnContextChanged()
    {
        wageTypes = null;
        wageTypeFactory = null;
    }

    protected override void OnDispose() =>
        wageTypes?.Dispose();

    private ItemCollection<RegulationWageType> LoadWageTypes()
    {
        var items = System.Threading.Tasks.Task.Run(WageTypeFactory.LoadPayrollItemsAsync).Result;
        return new ItemCollection<RegulationWageType>(items);
    }
}