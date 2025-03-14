﻿using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using MudBlazor;
using Blazored.LocalStorage;
using PayrollEngine.Client.Model;
using PayrollEngine.Client.Service;
using PayrollEngine.WebApp.Presentation;
using PayrollEngine.WebApp.Presentation.Case;
using PayrollEngine.WebApp.Presentation.Component;
using Case = PayrollEngine.WebApp.ViewModel.Case;
using Task = System.Threading.Tasks.Task;
using CaseChangeCaseValue = PayrollEngine.WebApp.ViewModel.CaseChangeCaseValue;

namespace PayrollEngine.WebApp.Server.Components.Pages;

public abstract partial class CasesPageBase(WorkingItems workingItems) : PageBase(workingItems)
{
    // external services
    [Inject]
    private IPayrollService PayrollService { get; set; }
    [Inject]
    private IConfiguration Configuration { get; set; }
    [Inject]
    private ILocalStorageService LocalStorage { get; set; }

    #region Base type

    /// <summary>
    /// The case type
    /// </summary>
    protected abstract CaseType CaseType { get; }

    /// <summary>
    /// The page name for new cases
    /// </summary>
    protected abstract string NewCasePageName { get; }

    /// <summary>
    /// The page title
    /// </summary>
    protected abstract string PageTitle { get; }

    /// <summary>
    /// The case changes grid id
    /// </summary>
    protected abstract string GridId { get; }

    /// <summary>
    /// The case value service
    /// </summary>
    protected abstract IBackendService<CaseChangeCaseValue, PayrollCaseChangeQuery> CaseValueBackendService { get; }

    /// <summary>
    /// The case document service
    /// </summary>
    protected abstract IBackendService<CaseDocument, Query> CaseDocumentBackendService { get; }

    #endregion

    #region Working items

    /// <summary>
    /// Update case values on tenant change
    /// </summary>
    protected override async Task OnTenantChangedAsync()
    {
        await base.OnTenantChangedAsync();
        await ChangePayrollAsync(null, null);
    }

    /// <summary>
    /// Update case values on payroll change
    /// </summary>
    /// <param name="payroll">The new payroll</param>
    protected override async Task OnPayrollChangedAsync(Payroll payroll)
    {
        await base.OnPayrollChangedAsync(payroll);
        await ChangePayrollAsync(payroll?.Id, Employee?.Id);
    }

    /// <summary>
    /// Update case values on employee change
    /// </summary>
    /// <param name="employee">The new employee</param>
    protected override async Task OnEmployeeChangedAsync(Employee employee)
    {
        await base.OnEmployeeChangedAsync(employee);
        await ChangePayrollAsync(Payroll?.Id, employee?.Id);
    }

    #endregion

    #region Grid

    private MudDataGrid<CaseChangeCaseValue> CaseValuesGrid { get; set; }

    /// <summary>
    /// Expand state at start
    /// </summary>
    private bool StartExpandGroups { get; set; }

    /// <summary>
    /// Grid dense mode
    /// <remarks>Based on the grid groups, dense is activated by default</remarks>
    /// </summary>
    private bool GridDense { get; set; } = true;

    /// <summary>
    /// The grid column configuration
    /// </summary>
    private List<GridColumnConfiguration> ColumnConfiguration =>
        GetColumnConfiguration(GridId);

    /// <summary>
    /// Toggle the grid dense state
    /// </summary>
    private async Task ToggleGridDenseAsync()
    {
        GridDense = !GridDense;

        // store dense mode
        await LocalStorage.SetItemAsBooleanAsync("CaseValueGridDenseMode", GridDense);
    }

    /// <summary>
    /// Initialize the grid
    /// </summary>
    private async Task InitGridAsync()
    {
        var denseMode = await LocalStorage.GetItemAsBooleanAsync("CaseValueGridDenseMode");
        if (denseMode.HasValue)
        {
            GridDense = denseMode.Value;
        }
    }

    #endregion

    #region Case

    private Color GetCaseColor(Case @case) =>
        @case.GetPriority(PageCulture).GetColor();

    private string GetCaseStyle(Case @case) =>
        @case.GetPriority(PageCulture).GetStyle(ThemeService);

    #endregion

    #region Case Values

    /// <summary>
    /// The working case values
    /// </summary>
    private List<CaseChangeCaseValue> CaseValues { get; set; } = [];

    /// <summary>
    /// Check if grid has any filter
    /// </summary>
    protected bool HasFilters =>
        CaseValuesGrid.FilterDefinitions.Any();

    /// <summary>
    /// Case grouping function
    /// </summary>
    private Func<CaseChangeCaseValue, object> CaseGroupBy { get; } =
        x => x.CaseChangeId;

    /// <summary>
    /// Case sorting function
    /// </summary>
    private Func<CaseChangeCaseValue, object> CaseSortBy { get; } =
        x => x.CaseChangeId;

    /// <summary>
    /// Case dense mode
    /// </summary>
    private bool CaseDense { get; set; } = true;

    /// <summary>
    /// Expand all case changes
    /// </summary>
    private void ExpandItemGroups() =>
        CaseValuesGrid?.ExpandAllGroups();

    /// <summary>
    /// Collapse all case changes
    /// </summary>
    private void CollapseItemGroups() =>
        CaseValuesGrid?.CollapseAllGroups();

    /// <summary>
    /// Reset the expand state
    /// </summary>
    protected void ResetItemGroups() =>
        CaseValuesGrid?.ExpandAllGroups();

    /// <summary>
    /// Reset all grid filters
    /// </summary>
    private async Task ResetFilterAsync() =>
        await CaseValuesGrid.ClearFiltersAsync();

    /// <summary>
    /// Apply case name filter
    /// </summary>
    /// <param name="caseName">The case name</param>
    private async Task ApplyFilterAsync(string caseName)
    {
        if (string.IsNullOrWhiteSpace(caseName))
        {
            return;
        }

        await CaseValuesGrid.SetColumnFilterAsync(
            nameof(CaseChangeCaseValue.CaseName),
            "equals",
            caseName);
    }

    /// <summary>
    /// Download excel file from unfiltered grid data
    /// </summary>
    private async Task ExcelDownloadAsync()
    {
        // server request
        var maxExport = Configuration.GetConfiguration<AppConfiguration>().ExcelExportMaxRecords;
        var state = CaseValuesGrid.BuildExportState(pageSize: maxExport);

        // retrieve all items, without any filter and sort
        var data = await GetServerDataAsync(state);
        var items = data.Items.ToList();
        if (!items.Any())
        {
            await UserNotification.ShowErrorMessageBoxAsync(Localizer, PageTitle, Localizer.Error.EmptyCollection);
            return;
        }

        try
        {
            await ExcelDownload.StartAsync(
                grid: CaseValuesGrid,
                items: items,
                jsRuntime: JsRuntime,
                name: DownloadTool.ToDownloadFileName(PageTitle));
            await UserNotification.ShowSuccessAsync(Localizer.Shared.DownloadCompleted);
        }
        catch (Exception exception)
        {
            Log.Error(exception, exception.GetBaseMessage());
            await UserNotification.ShowErrorMessageBoxAsync(Localizer, PageTitle, exception);
        }
    }

    /// <summary>
    /// Get case change value server data, handler for data grids
    /// </summary>
    /// <param name="state">The data grid state</param>
    /// <returns>Collection of items</returns>
    private async Task<GridData<CaseChangeCaseValue>> GetServerDataAsync(GridState<CaseChangeCaseValue> state)
    {
        try
        {
            var values = await CaseValueBackendService.QueryAsync();
            CaseValues = values.Items.ToList();
        }
        catch (Exception exception)
        {
            Log.Error(exception, exception.GetBaseMessage());
            await UserNotification.ShowErrorMessageBoxAsync(Localizer, PageTitle, exception);
        }
        try
        {
            return await CaseValueBackendService.QueryAsync(state);
        }
        catch (Exception exception)
        {
            Log.Error(exception, exception.GetBaseMessage());
            return new();
        }
    }

    /// <summary>
    /// Retrieve the case values
    /// </summary>
    private async Task SetupCaseValuesAsync()
    {
        try
        {
            var values = await CaseValueBackendService.QueryAsync();
            CaseValues = values.Items.ToList();
        }
        catch (Exception exception)
        {
            Log.Error(exception, exception.GetBaseMessage());
            await UserNotification.ShowErrorMessageBoxAsync(Localizer, PageTitle, exception);
        }
    }

    /// <summary>
    /// Toggle the case dense state
    /// </summary>
    private async Task ToggleCaseDenseAsync()
    {
        CaseDense = !CaseDense;

        // store dense mode
        await LocalStorage.SetItemAsBooleanAsync("CaseValueCaseDenseMode", CaseDense);
    }

    /// <summary>
    /// Initialize the grid
    /// </summary>
    private async Task InitCasesAsync()
    {
        var denseMode = await LocalStorage.GetItemAsBooleanAsync("CaseValueCaseDenseMode");
        if (denseMode.HasValue)
        {
            CaseDense = denseMode.Value;
        }
    }

    #endregion

    #region Undo Case

    /// <summary>
    /// Cancel the case
    /// </summary>
    /// <param name="caseChange">The case change</param>
    private async Task CancelCaseChangeAsync(CaseChangeCaseValue caseChange)
    {
        var caseChangeValues = CaseValues.Where(x => x.Id == caseChange.Id).ToList();
        if (!caseChangeValues.Any())
        {
            await UserNotification.ShowErrorMessageBoxAsync(Localizer, PageTitle, Localizer.CaseChange.EmptyCaseChange);
            return;
        }

        // dialog
        var parameters = new DialogParameters
        {
            { nameof(CaseUndoDialog.CaseChangeValues), caseChangeValues },
            { nameof(CaseUndoDialog.ValueFormatter), ValueFormatter },
            { nameof(CaseUndoDialog.Culture) , PageCulture }
        };
        var caseName = caseChangeValues.First().ValidationCaseName;
        var dialog = await (await DialogService.ShowAsync<CaseUndoDialog>(Localizer.CaseChange.UndoQuery(caseName), parameters)).Result;
        if (dialog == null || dialog.Canceled)
        {
            return;
        }

        // cancel case change setup
        var caseChangeSetup = new CaseChangeSetup
        {
            UserId = Session.User.Id,
            EmployeeId = Session.Employee.Id,
            CancellationId = caseChange.CaseChangeId,
            Case = new()
            {
                CaseName = caseChange.ValidationCaseName
            }
        };

        try
        {
            await PayrollService.AddCaseAsync<CaseChangeSetup, CaseChange>(new(Tenant.Id, Payroll.Id), caseChangeSetup);
            await UserNotification.ShowSuccessAsync(Localizer.CaseChange.UndoSuccess(caseChange.ValidationCaseName));

            // update case values
            await SetupCaseValuesAsync();
        }
        catch (Exception exception)
        {
            Log.Error(exception, exception.GetBaseMessage());
            await UserNotification.ShowErrorMessageBoxAsync(Localizer, Localizer.CaseChange.UndoError(caseChange.ValidationCaseName), exception);
        }
    }

    #endregion

    #region Documents

    /// <summary>
    /// Show the case field documents
    /// </summary>
    /// <param name="caseChange">The case change</param>
    private async Task ShowDocumentsAsync(CaseChangeCaseValue caseChange)
    {
        // documents
        var documents = await GetCaseDocumentsAsync(caseChange);
        if (documents == null)
        {
            return;
        }

        // dialog
        var parameters = new DialogParameters
        {
            { nameof(CaseDocumentsDialog<CaseDocument>.Documents), documents }
        };
        await DialogService.ShowAsync<CaseDocumentsDialog<CaseDocument>>(Localizer.Document.Documents, parameters);
    }

    /// <summary>
    /// Retrieve the case field documents
    /// </summary>
    /// <param name="caseChange">The case change</param>
    private async Task<ObservedHashSet<CaseDocument>> GetCaseDocumentsAsync(CaseChangeCaseValue caseChange)
    {
        try
        {
            var parameters = new Dictionary<string, object>
            {
                { nameof(CaseChangeCaseValue.CaseChangeId), caseChange.Id }
            };
            var documents = await CaseDocumentBackendService.QueryAsync(parameters: parameters);
            if (documents.Items.Any())
            {
                var documentList = new ObservedHashSet<CaseDocument>();
                await documentList.AddRangeAsync(documents.Items);
                return documentList;
            }
        }
        catch (Exception exception)
        {
            Log.Error(exception, exception.GetBaseMessage());
            await UserNotification.ShowErrorMessageBoxAsync(Localizer, PageTitle, exception);
        }
        return null;
    }

    #endregion

    #region Clusters

    private string ClusterAll => Localizer.Shared.All;

    /// <summary>
    /// The payroll clusters
    /// </summary>
    private List<string> PayrollClusters { get; set; }

    /// <summary>
    /// The filtered/working clusters
    /// </summary>
    private List<string> Clusters { get; set; }

    /// <summary>
    /// Test for filtered/working clusters
    /// </summary>
    private bool HasClusters => Clusters != null && Clusters.Any();

    /// <summary>
    /// The selected cluster
    /// </summary>
    private string SelectedCluster { get; set; }

    /// <summary>
    /// Setup payroll clusters
    /// </summary>
    private void SetupPayrollClusters()
    {
        if (Payroll == null || Payroll.ClusterSets == null || !Payroll.ClusterSets.Any())
        {
            return;
        }
        var clusters = Payroll.ClusterSets.Select(c => c.Name).ToList();
        PayrollClusters = clusters;
    }

    /// <summary>
    /// Setup filtered/working clusters
    /// </summary>
    private void SetupClusters()
    {
        if (PayrollClusters == null || !PayrollClusters.Any() ||
            PayrollAvailableCases == null || !PayrollAvailableCases.Any())
        {
            return;
        }

        // collect case clusters
        var uniqueClusters = new HashSet<string>();
        foreach (var cluster in PayrollClusters)
        {
            if (GetPayrollAvailableCases(cluster).Any())
            {
                uniqueClusters.Add(cluster);
            }
        }
        var clusters = uniqueClusters.ToList();
        // cluster all
        if (clusters.Any())
        {
            clusters.Insert(0, ClusterAll);
        }
        Clusters = clusters;
    }

    /// <summary>
    /// Handler for cluster change
    /// </summary>
    /// <param name="cluster">The selected cluster</param>
    private void SelectedClusterChanged(string cluster)
    {
        SetupAvailableCases(cluster);
        SelectedCluster = cluster;
    }

    /// <summary>
    /// Reset all clusters
    /// </summary>
    private void ResetClusters()
    {
        PayrollClusters = null;
        Clusters = null;
        SelectedCluster = null;
    }

    #endregion

    #region Available Cases

    /// <summary>
    /// The available case filter
    /// </summary>
    private string AvailableCaseFilter { get; set; }

    /// <summary>
    /// All available payroll cases
    /// </summary>
    private List<Case> PayrollAvailableCases { get; set; }

    /// <summary>
    /// Test for available payroll cases
    /// </summary>
    private bool HasPayrollAvailableCases =>
        PayrollAvailableCases != null && PayrollAvailableCases.Any();

    /// <summary>
    /// The filtered/working available cases
    /// </summary>
    private List<Case> AvailableCases { get; set; }

    /// <summary>
    /// Test for filtered/working available cases
    /// </summary>
    private bool HasAvailableCases =>
        AvailableCases != null && AvailableCases.Any();

    /// <summary>
    /// Build the clusters and cases after a payroll or employee change
    /// </summary>
    /// <param name="payrollId">The working payroll id</param>
    /// <param name="employeeId">The working employee id</param>
    private async Task ChangePayrollAsync(int? payrollId, int? employeeId)
    {
        // reset
        if (!payrollId.HasValue ||
            (CaseType == CaseType.Employee && !employeeId.HasValue))
        {
            ResetAvailableCases();
            ResetClusters();
            return;
        }

        // payroll clusters
        SetupPayrollClusters();

        // payroll cases
        await SetupPayrollAvailableCases(payrollId, employeeId);

        // available clusters
        SetupClusters();

        // available cases
        SetupAvailableCases(ClusterAll);

        // case values
        // the current solutions loads all case change values at the begin
        // the MudDataGrid has problems when using groups in combination with server data
        await SetupCaseValuesAsync();

        StartExpandGroups = true;
        StateHasChanged();
    }

    /// <summary>
    /// Setup payroll cases
    /// </summary>
    /// <param name="payrollId">The working payroll id</param>
    /// <param name="employeeId">The working employee id</param>
    private async Task SetupPayrollAvailableCases(int? payrollId, int? employeeId)
    {
        var payrollCases = new List<Case>();
        if (payrollId != null && WorkingItemsFulfilled(Tenant.Id, payrollId, employeeId))
        {
            try
            {
                var cases = await PayrollService.GetAvailableCasesAsync<Case>(
                    context: new(Tenant.Id, payrollId.Value),
                    userId: User.Id,
                    caseType: CaseType,
                    employeeId: employeeId,
                    // only visible cases
                    hidden: false);

                // order
                cases = cases.OrderBy(x => x.GetLocalizedName(PageCulture.Name)).ToList();
                payrollCases.AddRange(cases);
            }
            catch (Exception exception)
            {
                Log.Error(exception, exception.GetBaseMessage());
                await UserNotification.ShowErrorMessageBoxAsync(Localizer, PageTitle, exception);
            }
        }

        // available cases (filtering hidden cases)
        PayrollAvailableCases = !payrollCases.Any()
            ? []
            : payrollCases.Where(c => !(c.Attributes?.GetHidden(PageCulture) ?? false)).ToList();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="cluster"></param>
    /// <param name="filter"></param>
    private void SetupAvailableCases(string cluster = null, string filter = null)
    {
        IEnumerable<Case> cases = new List<Case>();

        // all available cases
        if (string.IsNullOrWhiteSpace(cluster) || string.Equals(cluster, ClusterAll))
        {
            cases = PayrollAvailableCases;
        }
        // validate cluster
        else if (Clusters != null && Clusters.Contains(cluster))
        {
            // cluster cases
            cases = GetPayrollAvailableCases(cluster);
        }

        // case filter
        if (!string.IsNullOrWhiteSpace(filter))
        {
            cases = cases.Where(x => x.IsMatching(filter, PageCulture.Name));
        }

        // case order
        cases = cases.OrderByDescending(x => x.GetPriority(PageCulture)).ThenBy(x => x.Name);

        AvailableCases = cases.ToList();
    }

    private IEnumerable<Case> GetPayrollAvailableCases(string cluster) =>
        PayrollAvailableCases.Where(x => x.Clusters != null && x.Clusters.Contains(cluster));

    private void StartCase(Case @case)
    {
        if (@case == null || !AvailableCases.Contains(@case))
        {
            return;
        }
        NavigateTo($"{NewCasePageName}/{@case.Name}");
    }

    private async Task SearchCaseAsync(Case @case)
    {
        if (@case == null)
        {
            return;
        }
        await ApplyFilterAsync(@case.GetLocalizedName(PageCulture.Name));
    }

    private void ResetAvailableCases()
    {
        PayrollAvailableCases = null;
        AvailableCases = null;
    }

    private void AvailableCaseFilterChangedAsync(string value)
    {
        AvailableCaseFilter = value;
        SetupAvailableCases(SelectedCluster, value);
    }

    #endregion

    #region Lifecycle

    protected override async Task OnPageInitializedAsync()
    {
        if (Session.Payroll != null)
        {
            await ChangePayrollAsync(Payroll.Id, Employee?.Id);
        }
        await base.OnPageInitializedAsync();
    }

    protected override async Task OnPageAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await InitCasesAsync();
            await InitGridAsync();
            if (StartExpandGroups)
            {
                ExpandItemGroups();
                StateHasChanged();
                StartExpandGroups = false;
            }
        }
        await base.OnPageAfterRenderAsync(firstRender);
    }

    #endregion

}