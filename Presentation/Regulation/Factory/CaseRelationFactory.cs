using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PayrollEngine.Client.Service;
using PayrollEngine.WebApp.ViewModel;

namespace PayrollEngine.WebApp.Presentation.Regulation.Factory;

public class CaseRelationFactory : ItemFactoryBase<RegulationCaseRelation>
{
    public ICaseRelationService CaseRelationService { get; set; }
    public IPayrollService PayrollService { get; set; }

    public CaseRelationFactory(ICaseRelationService caseRelationService, IPayrollService payrollService,
        Client.Model.Tenant tenant, Client.Model.Payroll payroll,
        List<Client.Model.Regulation> regulations) :
        base(tenant, payroll, regulations)
    {
        CaseRelationService = caseRelationService;
        PayrollService = payrollService;
    }

    public override async Task<List<RegulationCaseRelation>> QueryItems(Client.Model.Regulation regulation) =>
        await CaseRelationService.QueryAsync<RegulationCaseRelation>(new(Tenant.Id, regulation.Id));

    public override async Task<List<RegulationCaseRelation>> QueryPayrollItems() =>
        await PayrollService.GetCaseRelationsAsync<RegulationCaseRelation>(new(Tenant.Id, Payroll.Id));

    public override async Task<bool> SaveItem(ICollection<RegulationCaseRelation> caseRelations, RegulationCaseRelation caseRelation)
    {
        // regulation
        var regulation = Regulations?.FirstOrDefault();
        if (regulation == null)
        {
            return false;
        }

        // create or update
        if (caseRelation.Id == 0)
        {
            var createdCase = await CaseRelationService.CreateAsync(new(Tenant.Id, regulation.Id), caseRelation);
            SetCreatedData(createdCase, caseRelation);
        }
        else
        {
            await CaseRelationService.UpdateAsync(new(Tenant.Id, regulation.Id), caseRelation);
            SetUpdatedData(caseRelation);
        }

        // collection update
        return AddCollectionObject(caseRelations, caseRelation);
    }

    public override async Task<RegulationCaseRelation> DeleteItem(ICollection<RegulationCaseRelation> caseRelations,
        RegulationCaseRelation caseRelation)
    {
        // regulation
        var regulation = Regulations?.FirstOrDefault();
        if (caseRelation.Id <= 0 || regulation == null)
        {
            return null;
        }

        // delete
        await CaseRelationService.DeleteAsync(new(Tenant.Id, regulation.Id), caseRelation.Id);
        return DeleteCollectionObject(caseRelations, caseRelation);
    }
}