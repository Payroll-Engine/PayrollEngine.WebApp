using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PayrollEngine.Client.Service;
using PayrollEngine.WebApp.ViewModel;

namespace PayrollEngine.WebApp.Presentation.Regulation.Factory;

public class CaseFieldFactory(Client.Model.Tenant tenant, Client.Model.Payroll payroll,
        List<Client.Model.Regulation> regulations, IPayrollService payrollService,
        ICaseService caseService, ICaseFieldService caseFieldService)
    : ChildItemFactory<RegulationCase, RegulationCaseField>(tenant, payroll, regulations)
{
    private ICaseService CaseService { get; } = caseService;
    private ICaseFieldService CaseFieldService { get; } = caseFieldService;
    private IPayrollService PayrollService { get; } = payrollService;

    protected override async Task<List<RegulationCaseField>> QueryItemsAsync(Client.Model.Regulation regulation) =>
        await PayrollService.GetCaseFieldsAsync<RegulationCaseField>(new(Tenant.Id, Payroll.Id));

    public override async Task<List<RegulationCaseField>> QueryPayrollItemsAsync() =>
        await PayrollService.GetCaseFieldsAsync<RegulationCaseField>(new(Tenant.Id, Payroll.Id));

    public async Task<bool> SaveItem(ICollection<RegulationCaseField> caseFields, RegulationCaseField caseField)
    {
        // regulation
        var regulation = Regulations?.FirstOrDefault();
        if (regulation == null)
        {
            throw new PayrollException("Missing case field regulations.");
        }

        // case field
        var @case = caseField.Parent as RegulationCase;
        if (@case == null || @case.Id == 0)
        {
            throw new PayrollException($"Missing case for case field {caseField.Name}.");
        }

        // create or update
        if (caseField.Id == 0)
        {
            var createdCase = await CaseFieldService.CreateAsync(new(Tenant.Id, regulation.Id, @case.Id), caseField);
            SetCreatedData(createdCase, caseField);
        }
        else
        {
            await CaseFieldService.UpdateAsync(new(Tenant.Id, regulation.Id, @case.Id), caseField);
            SetUpdatedData(caseField);
        }

        // collection update
        return AddCollectionObject(caseFields, caseField);
    }

    public async Task<RegulationCaseField> DeleteItem(ICollection<RegulationCaseField> caseFields, RegulationCaseField caseField)
    {
        // regulation
        var regulation = Regulations?.FirstOrDefault();
        if (regulation == null)
        {
            throw new PayrollException("Missing case field regulations.");
        }

        // case field
        if (caseField.Id <= 0)
        {
            throw new PayrollException($"Unknown case field {caseField.Name} to delete.");
        }
        var @case = caseField.Parent as RegulationCase;
        if (@case == null || @case.Id == 0)
        {
            throw new PayrollException($"Missing case for case field {caseField.Name}.");
        }

        // delete
        await CaseFieldService.DeleteAsync(new(Tenant.Id, regulation.Id, @case.Id), caseField.Id);
        return DeleteCollectionObject(caseFields, caseField);
    }

    protected override async Task<List<RegulationCase>> QueryParentObjects(int tenantId, int regulationId) =>
        await CaseService.QueryAsync<RegulationCase>(new(tenantId, regulationId));

    protected override async Task<List<RegulationCaseField>> QueryChildObjects(int tenantId, int regulationId, RegulationCase parentObject)
    {
        return await CaseFieldService.QueryAsync<RegulationCaseField>(new(tenantId, regulationId, parentObject.Id));
    }
}