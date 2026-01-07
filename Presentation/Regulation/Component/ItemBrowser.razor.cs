using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;
using PayrollEngine.Client.Service;
using PayrollEngine.WebApp.ViewModel;
using Task = System.Threading.Tasks.Task;
using Tenant = PayrollEngine.Client.Model.Tenant;

namespace PayrollEngine.WebApp.Presentation.Regulation.Component;

public partial class ItemBrowser : IDisposable
{
    [Parameter]
    public RegulationEditContext EditContext { get; set; }
    [Parameter]
    public RegulationItemType ItemType { get; set; }
    [Parameter]
    public IRegulationItem SelectedItem { get; set; }
    [Parameter]
    public ItemGridConfig ItemGridConfig { get; set; }

    [Parameter]
    public bool ItemSelection { get; set; } = true;
    [Parameter]
    public EventCallback<IRegulationItem> SelectedItemChanged { get; set; }

    [Inject]
    private ICaseService CaseService { get; set; }
    [Inject]
    private ICaseFieldService CaseFieldService { get; set; }
    [Inject]
    private ICaseRelationService CaseRelationService { get; set; }
    [Inject]
    private ICollectorService CollectorService { get; set; }
    [Inject]
    private IWageTypeService WageTypeService { get; set; }
    [Inject]
    private IReportService ReportService { get; set; }
    [Inject]
    private IReportParameterService ReportParameterService { get; set; }
    [Inject]
    private IReportTemplateService ReportTemplateService { get; set; }
    [Inject]
    private ILookupService LookupService { get; set; }
    [Inject]
    private ILookupValueService LookupValueService { get; set; }
    [Inject]
    private IScriptService ScriptService { get; set; }
    [Inject]
    private IPayrollService PayrollService { get; set; }

    private List<Client.Model.Regulation> regulations;
    private Tenant Tenant => EditContext.Tenant;
    private Client.Model.Payroll Payroll => EditContext.Payroll;
    private List<Client.Model.Regulation> Regulations => EditContext.Regulations;

    #region Regulation Items

    // browsers
    private CaseBrowser CaseBrowser { get; set; }
    private CaseFieldBrowser CaseFieldBrowser { get; set; }
    private CaseRelationBrowser CaseRelationBrowser { get; set; }
    private CollectorBrowser CollectorBrowser { get; set; }
    private WageTypeBrowser WageTypeBrowser { get; set; }
    private ReportBrowser ReportBrowser { get; set; }
    private ReportParameterBrowser ReportParameterBrowser { get; set; }
    private ReportTemplateBrowser ReportTemplateBrowser { get; set; }
    private LookupBrowser LookupBrowser { get; set; }
    private LookupValueBrowser LookupValueBrowser { get; set; }
    private ScriptBrowser ScriptBrowser { get; set; }

    private ItemBrowserBase GetItemBrowser(RegulationItemType itemType)
    {
        switch (itemType)
        {
            case RegulationItemType.Case:
                return CaseBrowser;
            case RegulationItemType.CaseField:
                return CaseFieldBrowser;
            case RegulationItemType.CaseRelation:
                return CaseRelationBrowser;
            case RegulationItemType.Collector:
                return CollectorBrowser;
            case RegulationItemType.WageType:
                return WageTypeBrowser;
            case RegulationItemType.Report:
                return ReportBrowser;
            case RegulationItemType.ReportParameter:
                return ReportParameterBrowser;
            case RegulationItemType.ReportTemplate:
                return ReportTemplateBrowser;
            case RegulationItemType.Lookup:
                return LookupBrowser;
            case RegulationItemType.LookupValue:
                return LookupValueBrowser;
            case RegulationItemType.Script:
                return ScriptBrowser;
        }
        return null;
    }

    public async System.Threading.Tasks.Task<bool> SaveItem(IRegulationItem regulationItem)
    {
        if (regulationItem == null)
        {
            throw new ArgumentNullException(nameof(regulationItem));
        }

        // item browser
        var itemBrowser = GetItemBrowser(regulationItem.ItemType);
        if (itemBrowser == null)
        {
            throw new ArgumentNullException(nameof(regulationItem));
        }

        // save item
        return await itemBrowser.SaveAsync(regulationItem);
    }

    /// <summary>Delete a regulation item</summary>
    /// <param name="regulationItem">The object to delete</param>
    /// <returns>null: delete failed, the deleted regulation object or the base object of the deleted object</returns>
    /// <exception cref="ArgumentNullException"></exception>
    public async System.Threading.Tasks.Task<IRegulationItem> DeleteItem(IRegulationItem regulationItem)
    {
        if (regulationItem == null)
        {
            throw new ArgumentNullException(nameof(regulationItem));
        }

        // item browser
        var itemBrowser = GetItemBrowser(regulationItem.ItemType);
        if (itemBrowser == null)
        {
            throw new ArgumentNullException(nameof(regulationItem));
        }

        // save item
        return await itemBrowser.DeleteAsync(regulationItem);
    }

    public IRegulationItem DeriveItem(IRegulationItem regulationItem)
    {
        if (regulationItem == null)
        {
            throw new ArgumentNullException(nameof(regulationItem));
        }

        // regulation
        var regulation = Regulations.FirstOrDefault();
        if (regulation == null)
        {
            return null;
        }

        // new override instance
        if (Activator.CreateInstance(regulationItem.GetType()) is IRegulationItem newObject)
        {
            // inheritance
            regulationItem.ApplyInheritanceKeyTo(newObject);
            // base object
            newObject.BaseItem = regulationItem;
            // regulation
            newObject.RegulationName = regulation.Name;

            // change working object to the new override object
            return newObject;
        }

        return null;
    }

    private void SetupItemBrowsers()
    {
        CaseBrowser = new(Tenant, Payroll, Regulations, PayrollService, CaseService);
        CaseFieldBrowser = new(Tenant, Payroll, Regulations, PayrollService, CaseService, CaseFieldService);
        CaseRelationBrowser = new(Tenant, Payroll, Regulations, PayrollService, CaseRelationService);
        CollectorBrowser = new(Tenant, Payroll, Regulations, PayrollService, CollectorService);
        WageTypeBrowser = new(Tenant, Payroll, Regulations, PayrollService, WageTypeService);
        ReportBrowser = new(Tenant, Payroll, Regulations, PayrollService, ReportService);
        ReportParameterBrowser = new(Tenant, Payroll, Regulations, PayrollService, ReportService, ReportParameterService);
        ReportTemplateBrowser = new(Tenant, Payroll, Regulations, PayrollService, ReportService, ReportTemplateService);
        LookupBrowser = new(Tenant, Payroll, Regulations, PayrollService, LookupService);
        LookupValueBrowser = new(Tenant, Payroll, Regulations, PayrollService, LookupService, LookupValueService);
        ScriptBrowser = new(Tenant, Payroll, Regulations, PayrollService, ScriptService);
    }

    private void UpdateItemBrowsers()
    {
        foreach (var itemType in Enum.GetValues(typeof(RegulationItemType)))
        {
            GetItemBrowser((RegulationItemType)itemType).ChangeContext(EditContext);
        }
    }

    #endregion

    private async Task ChangeSelectedItem(IRegulationItem obj)
    {
        if (obj == SelectedItem)
        {
            return;
        }
        SelectedItem = obj;
        // parent notification
        await SelectedItemChanged.InvokeAsync(obj);
    }

    #region Lifecycle

    private RegulationEditContext lastEditContext;

    protected override async Task OnInitializedAsync()
    {
        lastEditContext = EditContext;
        SetupItemBrowsers();
        await base.OnInitializedAsync();
    }

    protected override async Task OnParametersSetAsync()
    {
        // context change
        var updateContext = lastEditContext != EditContext;

        // regulation change
        if (!CompareTool.EqualLists(regulations, Regulations))
        {
            regulations = Regulations;
            updateContext = true;
        }

        if (updateContext)
        {
            lastEditContext = EditContext;
            UpdateItemBrowsers();
        }

        await base.OnParametersSetAsync();
    }

    public void Dispose()
    {
        foreach (var itemType in Enum.GetValues(typeof(RegulationItemType)))
        {
            GetItemBrowser((RegulationItemType)itemType)?.Dispose();
        }
    }

    #endregion

}