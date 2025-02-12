using System;
using System.Linq;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using PayrollEngine.Client.Service;
using PayrollEngine.WebApp.Presentation;
using PayrollEngine.WebApp.Presentation.Component;
using PayrollEngine.WebApp.Presentation.Regulation;
using PayrollEngine.WebApp.Presentation.Regulation.Component;
using PayrollEngine.WebApp.ViewModel;
using Payroll = PayrollEngine.Client.Model.Payroll;
using Task = System.Threading.Tasks.Task;

namespace PayrollEngine.WebApp.Server.Components.Pages;

public partial class Regulation() : PageBase(WorkingItems.TenantChange | WorkingItems.PayrollChange)
{
    [Inject]
    private IPayrollService PayrollService { get; set; }

    private ItemBrowser ItemBrowser { get; set; }

    protected override async Task OnTenantChangedAsync()
    {
        ResetEditContext();
        await base.OnTenantChangedAsync();
    }

    protected override async Task OnPayrollChangedAsync(Payroll payroll)
    {
        await SetupEditContextAsync(payroll);
        await base.OnPayrollChangedAsync(payroll);
    }

    private bool HasRegulations { get; set; }

    // top regulation
    private string TopRegulationName
    {
        get
        {
            var name = Localizer.Shared.NotAvailable;
            var regulation = EditContext?.Regulations?.FirstOrDefault();
            if (regulation != null)
            {
                name = regulation.Name;
            }
            return name;
        }
    }

    #region Working Type

    private RegulationItemType WorkingType { get; set; }

    private Task SetWorkingType(RegulationItemType itemType)
    {
        if (WorkingType == itemType)
        {
            return Task.CompletedTask;
        }

        switch (itemType)
        {
            case RegulationItemType.Case:
            case RegulationItemType.CaseField:
                SelectedCaseType = itemType;
                break;
            case RegulationItemType.Report:
            case RegulationItemType.ReportParameter:
            case RegulationItemType.ReportTemplate:
                SelectedReportType = itemType;
                break;
            case RegulationItemType.Lookup:
            case RegulationItemType.LookupValue:
                SelectedLookupType = itemType;
                break;
        }

        WorkingType = itemType;
        // clear editing object
        SelectedItem = null;
        return Task.CompletedTask;
    }

    private string GetItemsLabel(RegulationItemType itemType) =>
        Localizer.GroupKey(itemType.ToString());

    // case
    private Variant CaseVariant =>
        WorkingType == SelectedCaseType ? Globals.ButtonAltVariant : Globals.ButtonVariant;
    private RegulationItemType SelectedCaseType { get; set; } = RegulationItemType.Case;

    // case relation
    private Variant CaseRelationVariant =>
        WorkingType == RegulationItemType.CaseRelation ? Globals.ButtonAltVariant : Globals.ButtonVariant;

    // collector
    private Variant CollectorVariant =>
        WorkingType == RegulationItemType.Collector ? Globals.ButtonAltVariant : Globals.ButtonVariant;

    // wage type
    private Variant WageTypeVariant =>
        WorkingType == RegulationItemType.WageType ? Globals.ButtonAltVariant : Globals.ButtonVariant;

    // report
    private Variant ReportVariant =>
        WorkingType == SelectedReportType ? Globals.ButtonAltVariant : Globals.ButtonVariant;
    private RegulationItemType SelectedReportType { get; set; } = RegulationItemType.Report;

    // lookup
    private Variant LookupVariant =>
        WorkingType == SelectedLookupType ? Globals.ButtonAltVariant : Globals.ButtonVariant;
    private RegulationItemType SelectedLookupType { get; set; } = RegulationItemType.Lookup;

    // script
    private Variant ScriptVariant =>
        WorkingType == RegulationItemType.Script ? Globals.ButtonAltVariant : Globals.ButtonVariant;

    #endregion

    #region Working Item

    private IRegulationItem SelectedItem { get; set; }

    private void ChangeSelectedItem(IRegulationItem item)
    {
        if (item != SelectedItem)
        {
            SelectedItem = item;
        }
    }

    private async Task SaveItem(object item)
    {
        if (item is not IRegulationItem regulationItem)
        {
            return;
        }

        try
        {
            if (await ItemBrowser.SaveItem(regulationItem))
            {
                ChangeSelectedItem(regulationItem);
                await UserNotification.ShowSuccessAsync(Localizer.Item.Saved(regulationItem.Name));
            }
        }
        catch (Exception exception)
        {
            Log.Error(exception, exception.GetBaseMessage());
            await UserNotification.ShowErrorMessageBoxAsync(Localizer, Localizer.Error.ItemUpdate(regulationItem.ItemName), exception);
        }
    }

    private async Task DeriveItem(object item)
    {
        if (item is not IRegulationItem regulationItem)
        {
            return;
        }

        try
        {
            var overrideObject = ItemBrowser.DeriveItem(regulationItem);
            if (overrideObject != null)
            {
                ChangeSelectedItem(overrideObject);
                await UserNotification.ShowInformationAsync(Localizer.Item.Derived(regulationItem.Name));
            }
        }
        catch (Exception exception)
        {
            Log.Error(exception, exception.GetBaseMessage());
            await UserNotification.ShowErrorMessageBoxAsync(Localizer, Localizer.Error.ItemDerive(regulationItem.ItemName), exception);
        }
    }

    private async Task DeleteItem(object item)
    {
        if (item is not IRegulationItem regulationItem)
        {
            return;
        }

        try
        {
            var deleteObject = await ItemBrowser.DeleteItem(regulationItem);
            if (deleteObject == null)
            {
                await UserNotification.ShowErrorAsync(Localizer.Error.ItemDelete(regulationItem.ItemName));
            }
            else
            {
                // deleted object has no base objects, clear the selection
                if (deleteObject.Id == regulationItem.Id)
                {
                    deleteObject = null;
                }
                ChangeSelectedItem(deleteObject);
                await UserNotification.ShowInformationAsync(Localizer.Item.Deleted(regulationItem.Name));
            }
        }
        catch (Exception exception)
        {
            Log.Error(exception, exception.GetBaseMessage());
            await UserNotification.ShowErrorMessageBoxAsync(Localizer, Localizer.Error.ItemDelete(regulationItem.ItemName), exception);
        }
    }

    #endregion

    #region Edit Context

    private RegulationEditContext EditContext { get; set; }

    private void ResetEditContext()
    {
        EditContext = null;
    }

    private async Task SetupEditContextAsync(Payroll payroll)
    {
        if (payroll == null)
        {
            ResetEditContext();
            return;
        }
        var regulations = await PayrollService.GetRegulationsAsync<Client.Model.Regulation>(
            new(Tenant.Id, payroll.Id));
        EditContext = new()
        {
            Tenant = Tenant,
            User = User,
            Payroll = payroll,
            Regulations = regulations,
            ActionProvider = new(TenantService, PayrollService, Tenant.Id, Payroll.Id)
        };
        HasRegulations = regulations.Any();
    }

    #endregion

    #region Lifecycle

    protected override async Task OnPageInitializedAsync()
    {
        if (Session.Payroll != null)
        {
            await SetupEditContextAsync(Session.Payroll);
        }
        await base.OnPageInitializedAsync();
    }

    #endregion

}