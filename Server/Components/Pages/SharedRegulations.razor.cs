using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using PayrollEngine.Client;
using PayrollEngine.Client.Service;
using PayrollEngine.WebApp.Presentation;
using PayrollEngine.Client.QueryExpression;
using PayrollEngine.WebApp.Presentation.Component;
using PayrollEngine.WebApp.Server.Components.Shared;
using PayrollEngine.WebApp.Server.Components.Dialogs;
using PayrollEngine.WebApp.Presentation.BackendService;
using ClientModel = PayrollEngine.Client.Model;

namespace PayrollEngine.WebApp.Server.Components.Pages;

public partial class SharedRegulations() : EditItemPageBase<ViewModel.RegulationShare, Query, RegulationShareDialog>(
    WorkingItems.None)
{
    [Inject]
    private IRegulationService RegulationService { get; set; }
    [Inject]
    private IDivisionService DivisionService { get; set; }
    [Inject]
    private RegulationShareBackendService RegulationShareBackendService { get; set; }
    [Inject]
    private PayrollHttpClient PayrollHttpClient { get; set; }

    protected override string GridId => GetTenantGridId(GridIdentifiers.Payrolls);
    protected override IBackendService<ViewModel.RegulationShare, Query> BackendService => RegulationShareBackendService;
    protected override ItemCollection<ViewModel.RegulationShare> Items { get; } = new();
    protected override string GetLocalizedItemName(bool plural) =>
        plural ? Localizer.RegulationShare.RegulationShares : Localizer.RegulationShare.RegulationShare;

    protected override async Task<bool> SetupDialogParametersAsync(DialogParameters parameters, ItemOperation operation, ViewModel.RegulationShare share)
    {
        // tenants
        var tenants = await GetTenantsAsync();
        if (!tenants.Any())
        {
            await UserNotification.ShowErrorMessageBoxAsync(
                Localizer,
                Localizer.RegulationShare.RegulationShare,
                Localizer.RegulationShare.MissingTenants);
            return false;
        }
        parameters.Add(nameof(RegulationShareDialog.Tenants), tenants);

        // divisions
        var divisions = await GetDivisionsAsync(tenants);
        if (!divisions.Any())
        {
            await UserNotification.ShowErrorMessageBoxAsync(
                Localizer,
                Localizer.RegulationShare.RegulationShare,
                Localizer.RegulationShare.MissingDivisions);
            return false;
        }
        parameters.Add(nameof(RegulationShareDialog.Divisions), divisions);

        // regulations
        var regulations = await GetSharedRegulationsAsync(tenants);
        if (!regulations.Any())
        {
            await UserNotification.ShowErrorMessageBoxAsync(
                Localizer,
                Localizer.RegulationShare.RegulationShare,
                Localizer.RegulationShare.MissingSharedRegulations);
            return false;
        }
        parameters.Add(nameof(RegulationShareDialog.SharedRegulations), regulations);

        // culture
        parameters.Add(nameof(RegulationShareDialog.Culture), PageCulture.Name);

        return await base.SetupDialogParametersAsync(parameters, operation, share);
    }

    /// <summary>
    /// Get shared regulations for any tenant
    /// </summary>
    /// <remarks>Tenant authentication must be disabled</remarks>
    private async Task<Dictionary<ClientModel.Tenant, List<ClientModel.Regulation>>> GetSharedRegulationsAsync(IEnumerable<ClientModel.Tenant> tenants)
    {
        var regulations = new Dictionary<ClientModel.Tenant, List<ClientModel.Regulation>>();
        var authTenant = PayrollHttpClient.GetTenantAuthorization();
        try
        {
            // disable tenant authorization
            PayrollHttpClient.RemoveTenantAuthorization();

            foreach (var tenant in tenants)
            {
                var tenantRegulations = await RegulationService.QueryAsync<ClientModel.Regulation>(
                    new(tenant.Id), new Query
                    {
                        Status = ObjectStatus.Active,
                        Filter = new Equals(nameof(ClientModel.Regulation.SharedRegulation), 1)
                    });
                if (tenantRegulations.Any())
                {
                    regulations.Add(tenant, tenantRegulations);
                }
            }
        }
        catch (Exception exception)
        {
            Log.Error(exception, exception.GetBaseMessage());
        }
        finally
        {
            if (!string.IsNullOrWhiteSpace(authTenant))
            {
                // restore tenant authorization
                PayrollHttpClient.SetTenantAuthorization(authTenant);
            }
        }
        return regulations;
    }

    /// <summary>
    /// Get divisions for any tenant
    /// </summary>
    /// <remarks>Tenant authentication must be disabled</remarks>
    private async Task<Dictionary<ClientModel.Tenant, List<ClientModel.Division>>> GetDivisionsAsync(IEnumerable<ClientModel.Tenant> tenants)
    {
        var divisions = new Dictionary<ClientModel.Tenant, List<ClientModel.Division>>();
        var authTenant = PayrollHttpClient.GetTenantAuthorization();
        try
        {
            // disable tenant authorization
            PayrollHttpClient.RemoveTenantAuthorization();

            foreach (var tenant in tenants)
            {
                var tenantDivisions = await DivisionService.QueryAsync<ClientModel.Division>(
                    new(tenant.Id), new Query
                    {
                        Status = ObjectStatus.Active
                    });
                if (tenantDivisions.Any())
                {
                    divisions.Add(tenant, tenantDivisions);
                }
            }
        }
        catch (Exception exception)
        {
            Log.Error(exception, exception.GetBaseMessage());
        }
        finally
        {
            if (!string.IsNullOrWhiteSpace(authTenant))
            {
                // restore tenant authorization
                PayrollHttpClient.SetTenantAuthorization(authTenant);
            }
        }
        return divisions;
    }

    private async Task<List<ClientModel.Tenant>> GetTenantsAsync() =>
        await TenantService.QueryAsync<ClientModel.Tenant>(new(), new()
        {
            Status = ObjectStatus.Active
        });
}