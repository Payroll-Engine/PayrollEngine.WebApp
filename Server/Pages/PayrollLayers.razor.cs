using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using PayrollEngine.Client.Service;
using PayrollEngine.WebApp.Presentation;
using PayrollEngine.WebApp.Presentation.BackendService;
using PayrollEngine.WebApp.Server.Dialogs;
using PayrollEngine.WebApp.Server.Shared;
using Payroll = PayrollEngine.Client.Model.Payroll;
using Task = System.Threading.Tasks.Task;
using PayrollLayer = PayrollEngine.WebApp.ViewModel.PayrollLayer;

namespace PayrollEngine.WebApp.Server.Pages;

public partial class PayrollLayers
{
    [Inject]
    private IRegulationService RegulationService { get; set; }
    [Inject]
    private PayrollLayerBackendService PayrollLayerBackendService { get; set; }

    private List<ViewModel.Regulation> Regulations { get; set; }
    protected override string GridId => GetTenantGridId(GridIdentifiers.PayrollLayers);
    protected override IBackendService<PayrollLayer, Query> BackendService => PayrollLayerBackendService;
    protected override ItemCollection<PayrollLayer> Items { get; } = new();

    public PayrollLayers() :
        base(WorkingItems.TenantChange | WorkingItems.PayrollChange)
    {
    }

    protected override async Task<bool> SetupDialogParametersAsync(DialogParameters parameters, ItemOperation operation)
    {
        // regulation names
        var regulationNames = Regulations.Select(x => x.Name).ToList();

        // editing regulation
        PayrollLayer excludeLayer = operation == ItemOperation.Edit ?
            parameters[ItemTypeName] as PayrollLayer : default;

        // remove used regulations (no double references)
        foreach (var layer in Items)
        {
            if (excludeLayer != null && layer.Id == excludeLayer.Id)
            {
                continue;
            }
            if (regulationNames.Contains(layer.RegulationName))
            {
                regulationNames.Remove(layer.RegulationName);
            }
        }
        if (!regulationNames.Any())
        {
            await DialogService.ShowMessageBox(ItemTypeUiName, Localizer.PayrollLayer.InvalidPayrollRegulation);
            return false;
        }
        parameters.Add(nameof(PayrollLayerDialog.RegulationNames), regulationNames);

        // payroll layers
        parameters.Add(nameof(PayrollLayerDialog.PayrollLayers), Items.ToList());

        return await base.SetupDialogParametersAsync(parameters, operation);
    }

    private string ValidateLayerOrder(PayrollLayer payrollLayer)
    {
        // regulation
        if (string.IsNullOrWhiteSpace(payrollLayer.RegulationName))
        {
            return Localizer.PayrollLayer.InvalidPayrollRegulation;
        }
        var regulation = Regulations.FirstOrDefault(r => string.Equals(r.Name, payrollLayer.RegulationName));
        if (regulation == null)
        {
            return Localizer.PayrollLayer.MissingRegulation(payrollLayer.RegulationName);
        }

        // regulation without base regulations
        if (regulation.BaseRegulations == null || !regulation.BaseRegulations.Any())
        {
            return string.Empty;
        }

        // base layers with a lower layer order
        var baseLayers = Items.Where(layer => layer.IsLowerLayerOrder(payrollLayer)).ToList();

        // base regulations
        foreach (var baseRegulation in regulation.BaseRegulations)
        {
            if (!baseLayers.Any(layer => string.Equals(layer.RegulationName, baseRegulation)))
            {
                return Localizer.PayrollLayer.MissingBaseRegulation(baseRegulation);
            }
        }
        return string.Empty;
    }

    private async Task SetupRegulationsAsync()
    {
        // missing payroll
        if (Payroll == null)
        {
            Regulations = new();
            return;
        }

        // tenant regulations
        var regulations = await RegulationService.QueryAsync<ViewModel.Regulation>(new(Tenant.Id));
        if (!regulations.Any())
        {
            Regulations = new();
            return;
        }

        // shared regulations
        var sharedRegulations = await TenantService.GetSharedRegulationsAsync<ViewModel.Regulation>(Tenant.Id, Payroll.DivisionId);
        foreach (var sharedRegulation in sharedRegulations)
        {
            if (sharedRegulations.Any())
            {
                if (regulations.Any(x => x.Id == sharedRegulation.Id))
                {
                    // prevent double regulations
                    continue;
                }
                regulations.Add(sharedRegulation);
            }
        }

        Regulations = regulations;

        // item setup
        var items = (await PayrollLayerBackendService.QueryAsync()).Items;
        Items.AddRange(items);
    }

    protected override async Task OnTenantChangedAsync()
    {
        await base.OnTenantChangedAsync();
        await SetupRegulationsAsync();
    }

    protected override async Task OnPayrollChangedAsync(Payroll payroll)
    {
        await base.OnPayrollChangedAsync(payroll);
        await SetupRegulationsAsync();
    }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        await SetupRegulationsAsync();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
    }
}