using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;
using PayrollEngine.Client.Service;
using PayrollEngine.WebApp.Presentation.Regulation.Factory;
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
    public int ItemsPageSize { get; set; } = 20;
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

    public async System.Threading.Tasks.Task<bool> SaveItem(IRegulationItem regulationItem)
    {
        if (regulationItem == null)
        {
            throw new ArgumentNullException(nameof(regulationItem));
        }

        switch (regulationItem.ItemType)
        {
            case RegulationItemType.Case:
                return await SaveCase(regulationItem as RegulationCase);
            case RegulationItemType.CaseField:
                return await SaveCaseField(regulationItem as RegulationCaseField);
            case RegulationItemType.CaseRelation:
                return await SaveCaseRelation(regulationItem as RegulationCaseRelation);
            case RegulationItemType.Collector:
                return await SaveCollector(regulationItem as RegulationCollector);
            case RegulationItemType.WageType:
                return await SaveWageType(regulationItem as RegulationWageType);
            case RegulationItemType.Report:
                return await SaveReport(regulationItem as RegulationReport);
            case RegulationItemType.ReportParameter:
                return await SaveReportParameter(regulationItem as RegulationReportParameter);
            case RegulationItemType.ReportTemplate:
                return await SaveReportTemplate(regulationItem as RegulationReportTemplate);
            case RegulationItemType.Lookup:
                return await SaveLookup(regulationItem as RegulationLookup);
            case RegulationItemType.LookupValue:
                return await SaveLookupValue(regulationItem as RegulationLookupValue);
            case RegulationItemType.Script:
                return await SaveScript(regulationItem as RegulationScript);
        }
        return false;
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

        switch (regulationItem.ItemType)
        {
            case RegulationItemType.Case:
                return await DeleteCase(regulationItem as RegulationCase);
            case RegulationItemType.CaseField:
                return await DeleteCaseField(regulationItem as RegulationCaseField);
            case RegulationItemType.CaseRelation:
                return await DeleteCaseRelation(regulationItem as RegulationCaseRelation);
            case RegulationItemType.Collector:
                return await DeleteCollector(regulationItem as RegulationCollector);
            case RegulationItemType.WageType:
                return await DeleteWageType(regulationItem as RegulationWageType);
            case RegulationItemType.Report:
                return await DeleteReport(regulationItem as RegulationReport);
            case RegulationItemType.ReportParameter:
                return await DeleteReportParameter(regulationItem as RegulationReportParameter);
            case RegulationItemType.ReportTemplate:
                return await DeleteReportTemplate(regulationItem as RegulationReportTemplate);
            case RegulationItemType.Lookup:
                return await DeleteLookup(regulationItem as RegulationLookup);
            case RegulationItemType.LookupValue:
                return await DeleteLookupValue(regulationItem as RegulationLookupValue);
            case RegulationItemType.Script:
                return await DeleteScript(regulationItem as RegulationScript);
        }
        return regulationItem;
    }

    public IRegulationItem OverrideItem(IRegulationItem regulationItem)
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
            newObject.RegulationId = regulation.Id;

            // change working object to the new override object
            return newObject;
        }

        return null;
    }

    #endregion

    #region Case

    private ItemCollection<RegulationCase> cases;
    protected ItemCollection<RegulationCase> Cases => cases ??= LoadCases();

    private CaseFactory caseFactory;
    protected CaseFactory CaseFactory => caseFactory ??=
        new(CaseService, PayrollService, Tenant, Payroll, Regulations);

    private ItemCollection<RegulationCase> LoadCases() =>
        new(Task.Run(CaseFactory.LoadPayrollItems).Result);

    private async System.Threading.Tasks.Task<bool> SaveCase(RegulationCase @case) =>
        await CaseFactory.SaveItem(cases, @case);

    private async System.Threading.Tasks.Task<IRegulationItem> DeleteCase(RegulationCase @case) =>
        await CaseFactory.DeleteItem(cases, @case);

    #endregion

    #region Case Field

    private ItemCollection<RegulationCaseField> caseFields;
    protected ItemCollection<RegulationCaseField> CaseFields => caseFields ??= LoadCaseFields();
    private CaseFieldFactory caseFieldFactory;
    protected CaseFieldFactory CaseFieldFactory => caseFieldFactory ??=
        new(CaseService, CaseFieldService, PayrollService, Tenant, Payroll, Regulations);

    private ItemCollection<RegulationCaseField> LoadCaseFields() =>
        new(Task.Run(CaseFieldFactory.LoadPayrollItems).Result);

    private async System.Threading.Tasks.Task<bool> SaveCaseField(RegulationCaseField caseField) =>
        await CaseFieldFactory.SaveItem(caseFields, caseField);

    private async System.Threading.Tasks.Task<IRegulationItem> DeleteCaseField(RegulationCaseField caseField) =>
        await CaseFieldFactory.DeleteItem(caseFields, caseField);

    #endregion

    #region Case Relation

    private ItemCollection<RegulationCaseRelation> caseRelations;
    protected ItemCollection<RegulationCaseRelation> CaseRelations => caseRelations ??= LoadCaseRelations();
    private CaseRelationFactory caseRelationFactory;
    protected CaseRelationFactory CaseRelationFactory => caseRelationFactory ??=
        new(CaseRelationService, PayrollService, Tenant, Payroll, Regulations);

    private ItemCollection<RegulationCaseRelation> LoadCaseRelations() =>
        new(Task.Run(CaseRelationFactory.LoadPayrollItems).Result);

    private async System.Threading.Tasks.Task<bool> SaveCaseRelation(RegulationCaseRelation caseRelation) =>
        await CaseRelationFactory.SaveItem(caseRelations, caseRelation);

    private async System.Threading.Tasks.Task<IRegulationItem> DeleteCaseRelation(RegulationCaseRelation caseRelation) =>
        await CaseRelationFactory.DeleteItem(caseRelations, caseRelation);

    #endregion

    #region Collector

    private ItemCollection<RegulationCollector> collectors;
    protected ItemCollection<RegulationCollector> Collectors => collectors ??= LoadCollectors();
    private CollectorFactory collectorFactory;
    protected CollectorFactory CollectorFactory => collectorFactory ??=
        new(CollectorService, PayrollService, Tenant, Payroll, Regulations);

    private ItemCollection<RegulationCollector> LoadCollectors() =>
        new(Task.Run(CollectorFactory.LoadPayrollItems).Result);

    private async System.Threading.Tasks.Task<bool> SaveCollector(RegulationCollector collector) =>
        await CollectorFactory.SaveItem(collectors, collector);

    private async System.Threading.Tasks.Task<IRegulationItem> DeleteCollector(RegulationCollector collector) =>
        await CollectorFactory.DeleteItem(collectors, collector);

    #endregion

    #region Wage Type

    private ItemCollection<RegulationWageType> wageTypes;
    protected ItemCollection<RegulationWageType> WageTypes => wageTypes ??= LoadWageTypes();
    private WageTypeFactory wageTypeFactory;
    protected WageTypeFactory WageTypeFactory => wageTypeFactory ??=
        new(WageTypeService, PayrollService, Tenant, Payroll, Regulations);

    private ItemCollection<RegulationWageType> LoadWageTypes() =>
        new(Task.Run(WageTypeFactory.LoadPayrollItems).Result);

    private async System.Threading.Tasks.Task<bool> SaveWageType(RegulationWageType wageType) =>
        await WageTypeFactory.SaveItem(wageTypes, wageType);

    private async System.Threading.Tasks.Task<IRegulationItem> DeleteWageType(RegulationWageType wageType) =>
        await WageTypeFactory.DeleteItem(wageTypes, wageType);

    #endregion

    #region Report

    private ItemCollection<RegulationReport> reports;
    protected ItemCollection<RegulationReport> Reports => reports ??= LoadReports();
    private ReportFactory reportFactory;
    protected ReportFactory ReportFactory => reportFactory ??=
        new(ReportService, PayrollService, Tenant, Payroll, Regulations);

    private ItemCollection<RegulationReport> LoadReports() =>
        new(Task.Run(ReportFactory.LoadPayrollItems).Result);

    private async System.Threading.Tasks.Task<bool> SaveReport(RegulationReport report) =>
        await ReportFactory.SaveItem(reports, report);

    private async System.Threading.Tasks.Task<IRegulationItem> DeleteReport(RegulationReport report) =>
        await ReportFactory.DeleteItem(reports, report);

    #endregion

    #region Report Parameter

    private ItemCollection<RegulationReportParameter> reportParameters;
    protected ItemCollection<RegulationReportParameter> ReportParameters => reportParameters ??= LoadReportParameters();
    private ReportParameterFactory reportParameterFactory;
    protected ReportParameterFactory ReportParameterFactory => reportParameterFactory ??=
        new(ReportService, ReportParameterService, PayrollService, Tenant, Payroll, Regulations);

    private ItemCollection<RegulationReportParameter> LoadReportParameters() =>
        new(Task.Run(ReportParameterFactory.LoadPayrollItems).Result);

    private async System.Threading.Tasks.Task<bool> SaveReportParameter(RegulationReportParameter reportParameter) =>
        await ReportParameterFactory.SaveItem(reportParameters, reportParameter);

    private async System.Threading.Tasks.Task<IRegulationItem> DeleteReportParameter(RegulationReportParameter reportParameter) =>
        await ReportParameterFactory.DeleteItem(reportParameters, reportParameter);

    #endregion

    #region Report Template

    private ItemCollection<RegulationReportTemplate> reportTemplates;
    protected ItemCollection<RegulationReportTemplate> ReportTemplates => reportTemplates ??= LoadReportTemplates();
    private ReportTemplateFactory reportTemplateFactory;
    protected ReportTemplateFactory ReportTemplateFactory => reportTemplateFactory ??=
        new(ReportService, ReportTemplateService, PayrollService, Tenant, Payroll, Regulations);

    private ItemCollection<RegulationReportTemplate> LoadReportTemplates() =>
        new(Task.Run(ReportTemplateFactory.LoadPayrollItems).Result);

    private async System.Threading.Tasks.Task<bool> SaveReportTemplate(RegulationReportTemplate reportTemplate) =>
        await ReportTemplateFactory.SaveItem(reportTemplates, reportTemplate);

    private async System.Threading.Tasks.Task<IRegulationItem> DeleteReportTemplate(RegulationReportTemplate reportTemplate) =>
        await ReportTemplateFactory.DeleteItem(reportTemplates, reportTemplate);

    #endregion

    #region Lookup

    private ItemCollection<RegulationLookup> lookups;
    protected ItemCollection<RegulationLookup> Lookups => lookups ??= LoadLookups();
    private LookupFactory lookupFactory;
    protected LookupFactory LookupFactory => lookupFactory ??=
        new(LookupService, PayrollService, Tenant, Payroll, Regulations);

    private ItemCollection<RegulationLookup> LoadLookups() =>
        new(Task.Run(LookupFactory.LoadPayrollItems).Result);

    private async System.Threading.Tasks.Task<bool> SaveLookup(RegulationLookup lookup) =>
        await LookupFactory.SaveItem(lookups, lookup);

    private async System.Threading.Tasks.Task<IRegulationItem> DeleteLookup(RegulationLookup lookup) =>
        await LookupFactory.DeleteItem(lookups, lookup);

    #endregion

    #region Lookup Value

    private ItemCollection<RegulationLookupValue> lookupValues;
    protected ItemCollection<RegulationLookupValue> LookupValues => lookupValues ??= LoadLookupValues();
    private LookupValueFactory lookupValueFactory;
    protected LookupValueFactory LookupValueFactory => lookupValueFactory ??=
        new(LookupService, LookupValueService, PayrollService, Tenant, Payroll, Regulations);

    private ItemCollection<RegulationLookupValue> LoadLookupValues() =>
        new(Task.Run(LookupValueFactory.LoadPayrollItems).Result);

    private async System.Threading.Tasks.Task<bool> SaveLookupValue(RegulationLookupValue lookupValue) =>
        await LookupValueFactory.SaveItem(lookupValues, lookupValue);

    private async System.Threading.Tasks.Task<IRegulationItem> DeleteLookupValue(RegulationLookupValue lookupValue) =>
        await LookupValueFactory.DeleteItem(lookupValues, lookupValue);

    #endregion

    #region Script

    private ItemCollection<RegulationScript> scripts;
    protected ItemCollection<RegulationScript> Scripts => scripts ??= LoadScripts();
    private ScriptFactory scriptFactory;
    protected ScriptFactory ScriptFactory => scriptFactory ??=
        new(ScriptService, PayrollService, Tenant, Payroll, Regulations);

    private ItemCollection<RegulationScript> LoadScripts() =>
        new(Task.Run(ScriptFactory.LoadPayrollItems).Result);

    private async System.Threading.Tasks.Task<bool> SaveScript(RegulationScript script) =>
        await ScriptFactory.SaveItem(scripts, script);

    private async System.Threading.Tasks.Task<IRegulationItem> DeleteScript(RegulationScript script) =>
        await ScriptFactory.DeleteItem(scripts, script);

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
        await base.OnInitializedAsync();
    }

    protected override async Task OnParametersSetAsync()
    {
        // tenant
        if (lastEditContext != EditContext)
        {
            lastEditContext = EditContext;
            ClearAllObjects();
        }

        // regulations
        if (!CompareTool.EqualLists(regulations, Regulations))
        {
            regulations = Regulations;
            ClearAllObjects();
        }

        await base.OnParametersSetAsync();
    }

    private void ClearAllObjects()
    {
        // case and case relation
        cases = null;
        caseFactory = null;

        caseFields = null;
        caseFactory = null;

        caseRelations = null;
        caseRelationFactory = null;

        // collector and wage type
        collectors = null;
        collectorFactory = null;

        wageTypes = null;
        wageTypeFactory = null;

        // report
        reports = null;
        reportFactory = null;

        reportParameters = null;
        reportParameterFactory = null;

        reportTemplates = null;
        reportTemplateFactory = null;

        // lookup
        lookups = null;
        lookupFactory = null;

        lookupValues = null;
        lookupValueFactory = null;

        // script
        scripts = null;
        scriptFactory = null;
    }

    public void Dispose()
    {
        cases?.Dispose();
        caseFields?.Dispose();
        caseRelations?.Dispose();
        collectors?.Dispose();
        wageTypes?.Dispose();
        reports?.Dispose();
        reportParameters?.Dispose();
        reportTemplates?.Dispose();
        lookups?.Dispose();
        lookupValues?.Dispose();
        scripts?.Dispose();
    }

    #endregion

}