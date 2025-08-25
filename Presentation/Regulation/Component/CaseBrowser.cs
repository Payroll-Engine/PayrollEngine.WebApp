using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PayrollEngine.Client.Service;
using PayrollEngine.WebApp.Presentation.Regulation.Factory;
using PayrollEngine.WebApp.ViewModel;
using Payroll = PayrollEngine.Client.Model.Payroll;
using Tenant = PayrollEngine.Client.Model.Tenant;

namespace PayrollEngine.WebApp.Presentation.Regulation.Component;

internal sealed class CaseBrowser : ItemBrowserBase
{
    internal CaseBrowser(Tenant tenant, Payroll payroll, List<Client.Model.Regulation> regulations,
        IPayrollService payrollService, ICaseService caseService) :
        base(tenant, payroll, regulations, payrollService)
    {
        CaseService = caseService ?? throw new ArgumentNullException(nameof(caseService));
    }

    private ItemCollection<RegulationCase> cases;
    private ICaseService CaseService { get; }
    private CaseFactory caseFactory;
    internal CaseFactory CaseFactory => caseFactory ??=
        new(Tenant, Payroll, Regulations, PayrollService, CaseService);

    internal ItemCollection<RegulationCase> Cases => 
        cases ??= LoadCases();
    internal override async Task<bool> SaveAsync(IRegulationItem item) =>
        await CaseFactory.SaveItem(cases, item as RegulationCase);
    internal override async Task<IRegulationItem> DeleteAsync(IRegulationItem item) =>
        await CaseFactory.DeleteItem(cases, item as RegulationCase);

    protected override void OnContextChanged()
    {
        cases = null;
        caseFactory = null;
    }

    protected override void OnDispose() =>
        cases?.Dispose();

    private ItemCollection<RegulationCase> LoadCases()
    {
        var items = System.Threading.Tasks.Task.Run(CaseFactory.LoadPayrollItemsAsync).Result;
        return new ItemCollection<RegulationCase>(items);
    }
}