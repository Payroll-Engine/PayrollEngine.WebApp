using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PayrollEngine.Client.Service;
using PayrollEngine.WebApp.Presentation.Regulation.Factory;
using PayrollEngine.WebApp.ViewModel;
using Payroll = PayrollEngine.Client.Model.Payroll;
using Task = System.Threading.Tasks.Task;
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
    private WageTypeFactory wageTypeFactory;
    private IWageTypeService WageTypeService { get; }

    internal override void Reset()
    {
        wageTypes = null;
        wageTypeFactory = null;
    }

    internal ItemCollection<RegulationWageType> WageTypes => wageTypes ??= LoadWageTypes();
    private WageTypeFactory WageTypeFactory => wageTypeFactory ??=
        new(Tenant, Payroll, Regulations, PayrollService, WageTypeService);
    internal override async Task<bool> SaveAsync(IRegulationItem item) =>
        await WageTypeFactory.SaveItem(wageTypes, item as RegulationWageType);

    internal override async Task<IRegulationItem> DeleteAsync(IRegulationItem item) =>
        await WageTypeFactory.DeleteItem(wageTypes, item as RegulationWageType);

    protected override void OnDispose() =>
        wageTypes?.Dispose();

    private ItemCollection<RegulationWageType> LoadWageTypes() =>
        new(Task.Run(WageTypeFactory.LoadPayrollItems).Result);
}