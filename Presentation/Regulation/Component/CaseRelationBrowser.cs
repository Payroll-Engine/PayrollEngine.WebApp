using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PayrollEngine.Client.Service;
using PayrollEngine.WebApp.Presentation.Regulation.Factory;
using PayrollEngine.WebApp.ViewModel;
using Payroll = PayrollEngine.Client.Model.Payroll;
using Tenant = PayrollEngine.Client.Model.Tenant;

namespace PayrollEngine.WebApp.Presentation.Regulation.Component;

internal class CaseRelationBrowser : ItemBrowserBase
{
    internal CaseRelationBrowser(Tenant tenant, Payroll payroll, List<Client.Model.Regulation> regulations,
        IPayrollService payrollService, ICaseRelationService caseRelationService) :
        base(tenant, payroll, regulations, payrollService)
    {
        CaseRelationService = caseRelationService ?? throw new ArgumentNullException(nameof(caseRelationService));
    }

    private ItemCollection<RegulationCaseRelation> caseRelations;
    private ICaseRelationService CaseRelationService { get; }
    private CaseRelationFactory caseRelationFactory;
    private CaseRelationFactory CaseRelationFactory => caseRelationFactory ??=
        new(Tenant, Payroll, Regulations, PayrollService, CaseRelationService);

    internal ItemCollection<RegulationCaseRelation> CaseRelations =>
        caseRelations ??= LoadCaseRelations();
    internal override async Task<bool> SaveAsync(IRegulationItem item) =>
        await CaseRelationFactory.SaveItem(caseRelations, item as RegulationCaseRelation);
    internal override async Task<IRegulationItem> DeleteAsync(IRegulationItem item) =>
        await CaseRelationFactory.DeleteItem(caseRelations, item as RegulationCaseRelation);

    protected override void OnContextChanged()
    {
        caseRelations = null;
        caseRelationFactory = null;
    }

    protected override void OnDispose() =>
        caseRelations?.Dispose();

    private ItemCollection<RegulationCaseRelation> LoadCaseRelations()
    {
        var items = System.Threading.Tasks.Task.Run(CaseRelationFactory.LoadPayrollItemsAsync).Result;
        return new ItemCollection<RegulationCaseRelation>(items);
    }
}