using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PayrollEngine.Client.Service;
using PayrollEngine.WebApp.Presentation.Regulation.Factory;
using PayrollEngine.WebApp.ViewModel;
using Payroll = PayrollEngine.Client.Model.Payroll;
using Tenant = PayrollEngine.Client.Model.Tenant;

namespace PayrollEngine.WebApp.Presentation.Regulation.Component;

internal sealed class CaseFieldBrowser : ItemBrowserBase
{
    internal CaseFieldBrowser(Tenant tenant, Payroll payroll, List<Client.Model.Regulation> regulations,
        IPayrollService payrollService, ICaseService caseService, ICaseFieldService caseFieldService) :
        base(tenant, payroll, regulations, payrollService)
    {
        CaseService = caseService ?? throw new ArgumentNullException(nameof(caseService));
        CaseFieldService = caseFieldService ?? throw new ArgumentNullException(nameof(caseFieldService));
    }

    private ItemCollection<RegulationCaseField> caseFields;
    private ICaseService CaseService { get; }
    private ICaseFieldService CaseFieldService { get; }
    private CaseFieldFactory caseFieldFactory;
    private CaseFieldFactory CaseFieldFactory => caseFieldFactory ??=
        new(Tenant, Payroll, Regulations, PayrollService, CaseService, CaseFieldService);

    internal ItemCollection<RegulationCaseField> CaseFields =>
        caseFields ??= LoadCaseFields();
    internal override async Task<bool> SaveAsync(IRegulationItem item) =>
        await CaseFieldFactory.SaveItem(caseFields, item as RegulationCaseField);
    internal override async Task<IRegulationItem> DeleteAsync(IRegulationItem item) =>
        await CaseFieldFactory.DeleteItem(caseFields, item as RegulationCaseField);

    protected override void OnContextChanged()
    {
        caseFields = null;
        caseFieldFactory = null;
    }

    protected override void OnDispose() =>
        caseFields?.Dispose();

    private ItemCollection<RegulationCaseField> LoadCaseFields()
    {
        var items = System.Threading.Tasks.Task.Run(CaseFieldFactory.LoadPayrollItemsAsync).Result;
        return new ItemCollection<RegulationCaseField>(items);
    }
}