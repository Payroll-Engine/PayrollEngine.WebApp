﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PayrollEngine.Client.Service;
using PayrollEngine.WebApp.ViewModel;

namespace PayrollEngine.WebApp.Presentation.Regulation.Factory;

public class LookupValueFactory(
    Client.Model.Tenant tenant,
    Client.Model.Payroll payroll,
    List<Client.Model.Regulation> regulations,
    IPayrollService payrollService,
    ILookupService caseService,
    ILookupValueService lookupValueService)
    : ChildItemFactory<RegulationLookup, RegulationLookupValue>(tenant, payroll, regulations)
{
    private ILookupService LookupService { get; } = caseService;
    private ILookupValueService LookupValueService { get; } = lookupValueService;
    private IPayrollService PayrollService { get; } = payrollService;

    protected override async Task<List<RegulationLookupValue>> QueryItemsAsync(Client.Model.Regulation regulation) =>
        await PayrollService.GetLookupValuesAsync<RegulationLookupValue>(new(Tenant.Id, Payroll.Id));

    public override async Task<List<RegulationLookupValue>> QueryPayrollItemsAsync() =>
        await PayrollService.GetLookupValuesAsync<RegulationLookupValue>(new(Tenant.Id, Payroll.Id));

    public async Task<bool> SaveItem(ICollection<RegulationLookupValue> lookupValues,
        RegulationLookupValue lookupValue)
    {
        // regulation
        var regulation = Regulations?.FirstOrDefault();
        if (regulation == null)
        {
            return false;
        }

        // lookup
        var lookup = lookupValue.Parent as RegulationLookup;
        if (lookup == null || lookup.Id == 0)
        {
            return false;
        }

        // create or update
        if (lookupValue.Id == 0)
        {
            var createdCase = await LookupValueService.CreateAsync(new(Tenant.Id, regulation.Id, lookup.Id), lookupValue);
            SetCreatedData(createdCase, lookupValue);
        }
        else
        {
            await LookupValueService.UpdateAsync(new(Tenant.Id, regulation.Id, lookup.Id), lookupValue);
            SetUpdatedData(lookupValue);
        }

        // collection update
        return AddCollectionObject(lookupValues, lookupValue);
    }

    public async Task<RegulationLookupValue> DeleteItem(ICollection<RegulationLookupValue> lookupValues, RegulationLookupValue lookupValue)
    {
        // regulation
        var regulation = Regulations?.FirstOrDefault();
        if (lookupValue.Id <= 0 || regulation == null)
        {
            return null;
        }

        // lookup
        var lookup = lookupValue.Parent as RegulationLookup;
        if (lookup == null || lookup.Id == 0)
        {
            return null;
        }

        // delete
        await LookupValueService.DeleteAsync(new(Tenant.Id, regulation.Id, lookup.Id), lookupValue.Id);
        return DeleteCollectionObject(lookupValues, lookupValue);
    }

    protected override async Task<List<RegulationLookup>> QueryParentObjects(int tenantId, int regulationId) =>
        await LookupService.QueryAsync<RegulationLookup>(new(tenantId, regulationId));

    protected override async Task<List<RegulationLookupValue>> QueryChildObjects(int tenantId, int regulationId, RegulationLookup parentObject) =>
        await LookupValueService.QueryAsync<RegulationLookupValue>(new(tenantId, regulationId, parentObject.Id));

}