using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using PayrollEngine.Client.Model;
using PayrollEngine.Client.Service;
using Task = System.Threading.Tasks.Task;
using CaseChangeCaseValue = PayrollEngine.WebApp.ViewModel.CaseChangeCaseValue;
using PayrollEngine.WebApp.Presentation;
using PayrollEngine.WebApp.Presentation.Case;
using Blazored.LocalStorage;
using Case = PayrollEngine.WebApp.ViewModel.Case;

namespace PayrollEngine.WebApp.Server.Pages;

public abstract partial class CasesPageBase
{
    // external services
    [Inject]
    private IPayrollService PayrollService { get; set; }
    [Inject]
    private ILocalStorageService LocalStorage { get; set; }

    protected CasesPageBase(WorkingItems workingItems) :
        base(workingItems)
    {
    }

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
    /// <param name="tenant">The new tenant</param>
    protected override async Task OnTenantChangedAsync(Tenant tenant)
    {
        await base.OnTenantChangedAsync(tenant);
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

    protected MudDataGrid<CaseChangeCaseValue> CaseValuesGrid { get; set; }

    /// <summary>
    /// Expand state at start
    /// </summary>
    private bool StartExpandGroups { get; set; }

    /// <summary>
    /// Dense mode
    /// <remarks>Based on the grid groups, dense is activated by default</remarks>
    /// </summary>
    protected bool Dense { get; set; } = true;

    /// <summary>
    /// The grid column configuration
    /// </summary>
    protected List<GridColumnConfiguration> ColumnConfiguration =>
        GetColumnConfiguration(GridId);

    /// <summary>
    /// Toggle the grid dense state
    /// </summary>
    protected async Task ToggleGridDenseAsync()
    {
        Dense = !Dense;

        // store dense mode
        await LocalStorage.SetItemAsBooleanAsync("CaseValueDenseMode", Dense);
    }

    /// <summary>
    /// Setup the grid
    /// </summary>
    private async Task SetupGridAsync()
    {
        var denseMode = await LocalStorage.GetItemAsBooleanAsync("CaseValueDenseMode");
        if (denseMode.HasValue)
        {
            Dense = denseMode.Value;
        }
    }

    #endregion

    #region Case Values

    /// <summary>
    /// The working case values
    /// </summary>
    protected List<CaseChangeCaseValue> CaseValues { get; private set; } = new();

    /// <summary>
    /// Check if grid has any filter
    /// </summary>
    protected bool HasFilters =>
        CaseValuesGrid.FilterDefinitions.Any();

    /// <summary>
    /// Case grouping function
    /// </summary>
    protected Func<CaseChangeCaseValue, object> CaseGroupBy { get; set; } =
        x => x.CaseChangeId;

    /// <summary>
    /// Case sorting function
    /// </summary>
    protected Func<CaseChangeCaseValue, object> CaseSortBy { get; set; } =
        x => x.CaseChangeId;

    /// <summary>
    /// Expand all case changes
    /// </summary>
    protected void ExpandItemGroups() =>
        CaseValuesGrid?.ExpandAllGroups();

    /// <summary>
    /// Collapse all case changes
    /// </summary>
    protected void CollapseItemGroups() =>
        CaseValuesGrid?.CollapseAllGroups();

    /// <summary>
    /// Reset the expand state
    /// </summary>
    protected void ResetItemGroups() =>
        CaseValuesGrid?.ExpandAllGroups();

    /// <summary>
    /// Reset all grid filters
    /// </summary>
    protected async Task ResetFilterAsync() =>
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
            await UserNotification.ShowErrorMessageBoxAsync("Case values read error", exception);
        }
    }

    #endregion

    #region Undo Case

    /// <summary>
    /// Cancel the case
    /// </summary>
    /// <param name="caseChange">The case change</param>
    protected async Task CancelCaseChangeAsync(CaseChangeCaseValue caseChange)
    {
        var caseChangeValues = CaseValues.Where(x => x.Id == caseChange.Id).ToList();
        if (!caseChangeValues.Any())
        {
            await UserNotification.ShowErrorMessageBoxAsync("Case undo update error", "Case change without values");
            return;
        }

        // dialog
        var parameters = new DialogParameters
        {
            { nameof(CaseUndoDialog.CaseChangeValues), caseChangeValues },
            { nameof(CaseUndoDialog.ValueFormatter), ValueFormatter },
            { nameof(CaseUndoDialog.Language) , UserLanguage }
        };
        var caseName = caseChangeValues.First().ValidationCaseName;
        var dialog = await (await DialogService.ShowAsync<CaseUndoDialog>($"Undo case {caseName}", parameters)).Result;
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
            await UserNotification.ShowSuccessAsync($"Undo successful for Case {caseChange.ValidationCaseName}");

            // update case values
            await SetupCaseValuesAsync();
        }
        catch (Exception exception)
        {
            Log.Error(exception, exception.GetBaseMessage());
            await UserNotification.ShowErrorMessageBoxAsync("Case undo update error", exception);
        }
    }

    #endregion

    #region Documents

    /// <summary>
    /// Show the case field documents
    /// </summary>
    /// <param name="caseChange">The case change</param>
    protected async Task ShowDocumentsAsync(CaseChangeCaseValue caseChange)
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
        await DialogService.ShowAsync<CaseDocumentsDialog<CaseDocument>>(caseChange.CaseFieldName.ToPascalSentence().EnsureEnd(" documents"), parameters);
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
            await UserNotification.ShowErrorMessageBoxAsync("Case values read error", exception);
        }
        return null;
    }

    /// <summary>
    /// Create new case change query
    /// </summary>
    /// <returns></returns>
    protected virtual PayrollCaseChangeQuery NewCaseChangeQuery()
    {
        var query = new PayrollCaseChangeQuery
        {
            CaseType = CaseType,
            UserId = Session.User.Id,
            Language = Session.User.Language
        };
        if (HasPayroll)
        {
            query.DivisionId = Payroll.DivisionId;
        }
        return query;
    }

    #endregion

    #region Clusters

    private const string ClusterAll = "All";

    /// <summary>
    /// The payroll clusters
    /// </summary>
    public List<string> PayrollClusters { get; set; }

    /// <summary>
    /// The filtered/working clusters
    /// </summary>
    public List<string> Clusters { get; set; }

    /// <summary>
    /// Test for filtered/working clusters
    /// </summary>
    public bool HasClusters => Clusters != null && Clusters.Any();

    /// <summary>
    /// The selected cluster
    /// </summary>
    public MudChip SelectedCluster { get; set; }

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

        var clusters = new List<string>();
        // exclude empty clusters
        foreach (var cluster in PayrollClusters)
        {
            if (GetPayrollAvailableCases(cluster).Any())
            {
                clusters.Add(cluster);
            }
        }
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
    private void SelectedClusterChanged(MudChip cluster)
    {
        SetupAvailableCases(cluster?.Text);
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
    protected string AvailableCaseFilter { get; set; }

    /// <summary>
    /// All available payroll cases
    /// </summary>
    protected List<Case> PayrollAvailableCases { get; private set; }

    /// <summary>
    /// Test for available payroll cases
    /// </summary>
    protected bool HasPayrollAvailableCases =>
        PayrollAvailableCases != null && PayrollAvailableCases.Any();

    /// <summary>
    /// The filtered/working available cases
    /// </summary>
    protected List<Case> AvailableCases { get; private set; }

    /// <summary>
    /// Test for filtered/working available cases
    /// </summary>
    protected bool HasAvailableCases =>
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
                //var valueFormatter = ValueFormatter;
                var cases = await PayrollService.GetAvailableCasesAsync<Case>(
                    new(Tenant.Id, payrollId.Value), User.Id,
                    CaseType, employeeId: employeeId);

                // order
                cases = cases.OrderBy(x => x.GetLocalizedName(UserLanguage)).ToList();
                payrollCases.AddRange(cases);
            }
            catch (Exception exception)
            {
                Log.Error(exception, exception.GetBaseMessage());
                await UserNotification.ShowErrorMessageBoxAsync("Case read error", exception);
            }
        }

        // available cases (filtering hidden cases)
        PayrollAvailableCases = !payrollCases.Any()
            ? new()
            : payrollCases.Where(c => !(c.Attributes?.GetHidden(UserLanguage) ?? false)).ToList();
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
            cases = cases.Where(x => x.IsMatching(filter, UserLanguage));
        }
        AvailableCases = cases.ToList();
    }

    private IEnumerable<Case> GetPayrollAvailableCases(string cluster) =>
        PayrollAvailableCases.Where(x => x.Clusters != null && x.Clusters.Contains(cluster));

    protected void StartCase(Case @case)
    {
        if (@case == null || !AvailableCases.Contains(@case))
        {
            return;
        }
        NavigateTo($"{NewCasePageName}/{@case.Name}");
    }

    protected async Task SearchCaseAsync(Case @case)
    {
        if (@case == null)
        {
            return;
        }
        await ApplyFilterAsync(@case.GetLocalizedName(UserLanguage));
    }

    private void ResetAvailableCases()
    {
        PayrollAvailableCases = null;
        AvailableCases = null;
    }

    private void AvailableCaseFilterChangedAsync(string value)
    {
        AvailableCaseFilter = value;
        SetupAvailableCases(SelectedCluster?.Text, value);
    }

    #endregion

    #region Lifecycle

    protected override async Task OnInitializedAsync()
    {
        await SetupGridAsync();
        if (Session.Payroll != null)
        {
            await ChangePayrollAsync(Payroll.Id, Employee?.Id);
        }
        await base.OnInitializedAsync();
    }

    protected override Task OnAfterRenderAsync(bool firstRender)
    {
        if (StartExpandGroups)
        {
            ExpandItemGroups();
            StateHasChanged();
            StartExpandGroups = false;
        }
        return base.OnAfterRenderAsync(firstRender);
    }

    #endregion

}