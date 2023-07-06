using System;
using System.Linq;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using PayrollEngine.Client.Service;
using PayrollEngine.WebApp.Presentation.Regulation;
using PayrollEngine.WebApp.Presentation.Regulation.Component;
using PayrollEngine.WebApp.Presentation.Regulation.Editor;
using PayrollEngine.WebApp.ViewModel;
using Payroll = PayrollEngine.Client.Model.Payroll;
using Task = System.Threading.Tasks.Task;

namespace PayrollEngine.WebApp.Server.Pages;

public partial class Regulation
{
    [Inject]
    private IPayrollService PayrollService { get; set; }

    private ItemBrowser ItemBrowser { get; set; }
    private ItemEditorPanel ItemEditorPanel { get; set; }

    public Regulation() :
        base(WorkingItems.TenantChange | WorkingItems.PayrollChange)
    {
    }

    protected override async Task OnTenantChangedAsync()
    {
        ResetEditContext();
        await base.OnTenantChangedAsync();
    }

    protected override async Task OnPayrollChangedAsync(Payroll payroll)
    {
        SetupEditContext(payroll);
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
        WorkingType == SelectedCaseType ? Variant.Filled : Variant.Outlined;
    private RegulationItemType SelectedCaseType { get; set; } = RegulationItemType.Case;
    private void CaseSelected() =>
        WorkingType = SelectedCaseType;

    // case relation
    private Variant CaseRelationVariant =>
        WorkingType == RegulationItemType.CaseRelation ? Variant.Filled : Variant.Outlined;

    // collector
    private Variant CollectorVariant =>
        WorkingType == RegulationItemType.Collector ? Variant.Filled : Variant.Outlined;

    // wage type
    private Variant WageTypeVariant =>
        WorkingType == RegulationItemType.WageType ? Variant.Filled : Variant.Outlined;

    // report
    private Variant ReportVariant =>
        WorkingType == SelectedReportType ? Variant.Filled : Variant.Outlined;
    private RegulationItemType SelectedReportType { get; set; } = RegulationItemType.Report;
    private void ReportSelected() =>
        WorkingType = SelectedReportType;

    // lookup
    private Variant LookupVariant =>
        WorkingType == SelectedLookupType ? Variant.Filled : Variant.Outlined;
    private RegulationItemType SelectedLookupType { get; set; } = RegulationItemType.Lookup;
    private void LookupSelected() =>
        WorkingType = SelectedLookupType;

    // script
    private Variant ScriptVariant =>
        WorkingType == RegulationItemType.Script ? Variant.Filled : Variant.Outlined;

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
        try
        {
            if (item is IRegulationItem regulationItem && await ItemBrowser.SaveItem(regulationItem))
            {
                ChangeSelectedItem(regulationItem);
                await UserNotification.ShowSuccessAsync($"{GetItemLocalizedName(regulationItem)} saved");
            }
        }
        catch (Exception exception)
        {
            Log.Error(exception, exception.GetBaseMessage());
            await UserNotification.ShowErrorMessageBoxAsync(Localizer, "Save Error", exception);
        }
    }

    private async Task DeriveItem(object item)
    {
        try
        {
            if (item is IRegulationItem regulationItem)
            {
                var overrideObject = ItemBrowser.DeriveItem(regulationItem);
                if (overrideObject != null)
                {
                    ChangeSelectedItem(overrideObject);
                    await UserNotification.ShowInformationAsync($"{GetItemLocalizedName(regulationItem)} created");
                }
            }
        }
        catch (Exception exception)
        {
            Log.Error(exception, exception.GetBaseMessage());
            await UserNotification.ShowErrorMessageBoxAsync(Localizer, "Override Error", exception);
        }
    }

    private async Task DeleteItem(object item)
    {
        try
        {
            if (item is IRegulationItem regulationItem)
            {
                var deleteObject = await ItemBrowser.DeleteItem(regulationItem);
                if (deleteObject == null)
                {
                    await UserNotification.ShowErrorAsync(Localizer.Error.DeleteFailed);
                }
                else
                {
                    // deleted object has no base objects, clear the selection
                    if (deleteObject.Id == regulationItem.Id)
                    {
                        deleteObject = null;
                    }
                    ChangeSelectedItem(deleteObject);
                    await UserNotification.ShowInformationAsync(Localizer.Item.Deleted(GetItemLocalizedName(regulationItem)));
                }
            }
        }
        catch (Exception exception)
        {
            Log.Error(exception, exception.GetBaseMessage());
            await UserNotification.ShowErrorMessageBoxAsync(Localizer, Localizer.Error.DeleteFailed, exception);
        }
    }

    private string GetItemLocalizedName(IRegulationItem regulationItem) =>
        Localizer.GroupKey(regulationItem.ItemType.ToString());

    #endregion

    #region Edit Context

    private RegulationEditContext EditContext { get; set; }

    private void ResetEditContext()
    {
        EditContext = null;
    }

    private void SetupEditContext(Payroll payroll)
    {
        if (payroll == null)
        {
            ResetEditContext();
            return;
        }
        var regulations = Task.Run(() =>
            PayrollService.GetRegulationsAsync<Client.Model.Regulation>(new(Tenant.Id, payroll.Id))).Result;
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

    protected override async Task OnInitializedAsync()
    {
        SetupEditContext(Payroll);
        await base.OnInitializedAsync();
    }

}