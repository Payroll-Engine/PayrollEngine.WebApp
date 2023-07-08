using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Blazored.LocalStorage;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using PayrollEngine.Client.Service;
using PayrollEngine.Client.QueryExpression;
using PayrollEngine.WebApp.ViewModel;
using PayrollEngine.WebApp.Presentation.BackendService;
using PayrollEngine.WebApp.Server.Shared;
using Task = System.Threading.Tasks.Task;
using Microsoft.Extensions.Configuration;
using PayrollEngine.WebApp.Presentation;
using PayrollEngine.WebApp.Presentation.Component;

namespace PayrollEngine.WebApp.Server.Pages;

public partial class PayrunResults
{
    [Parameter]
    public string Payrun { get; set; }

    [Inject]
    private IPayrunService PayrunService { get; set; }
    [Inject]
    private PayrollResultBackendService PayrollResultBackendService { get; set; }
    [Inject]
    private IConfiguration Configuration { get; set; }
    [Inject]
    private ILocalStorageService LocalStorage { get; set; }
    [Inject]
    private IJSRuntime JsRuntime { get; set; }

    public PayrunResults() :
        base(WorkingItems.TenantChange | WorkingItems.PayrollChange | WorkingItems.EmployeeChange)
    {
    }

    /// <inheritdoc />
    protected override async Task OnTenantChangedAsync()
    {
        await SetupPage();
        await base.OnTenantChangedAsync();
    }

    /// <inheritdoc />
    protected override async Task OnPayrollChangedAsync(Client.Model.Payroll payroll)
    {
        await SetupPage();
        await base.OnPayrollChangedAsync(payroll);
    }

    /// <inheritdoc />
    protected override async Task OnEmployeeChangedAsync(Client.Model.Employee employee)
    {
        await RefreshServerDataAsync();
        await base.OnEmployeeChangedAsync(employee);
    }

    #region Payruns

    /// <summary>
    /// The payruns of the working payroll
    /// </summary>
    private List<Payrun> Payruns { get; set; }

    /// <summary>
    /// True if payroll contains payruns
    /// </summary>
    private bool HasPayruns => Payruns != null && Payruns.Any();

    /// <summary>
    /// Selected payrun
    /// </summary>
    private Payrun SelectedPayrun { get; set; }

    /// <summary>
    /// Selected payrun by name
    /// </summary>
    protected string SelectedPayrunName
    {
        get => SelectedPayrun?.Name;
        set => throw new NotSupportedException();
    }

    /// <summary>
    /// Setup payruns of the working payroll
    /// </summary>
    /// <returns></returns>
    private async Task SetupPayrunsAsync()
    {
        try
        {
            // retrieve payruns by payroll
            Query query = new()
            {
                Filter = new Equals(nameof(ViewModel.Payrun.PayrollId), Payroll.Id)
            };
            var payruns =
                HasPayroll ?
                await PayrunService.QueryAsync<Payrun>(new(Tenant.Id), query) :
                null;
            if (payruns == null)
            {
                Payruns = null;
                SelectedPayrun = null;
                return;
            }

            // payrun collection
            Payruns = payruns.Select(x => new Payrun(x)).ToList();

            // selected payrun
            Payrun selected = null;
            // page parameter on startup
            if (!Initialized && !string.IsNullOrWhiteSpace(Payrun))
            {
                selected = Payruns.FirstOrDefault(x =>
                    string.Equals(x.Name, Payrun, StringComparison.InvariantCultureIgnoreCase));
            }
            // payroll with single payrun
            if (selected == null && payruns.Count == 1)
            {
                selected = payruns.First();
            }
            SelectedPayrun = selected;
        }
        catch (Exception exception)
        {
            Log.Error(exception, exception.GetBaseMessage());
            if (Initialized)
            {
                await UserNotification.ShowErrorMessageBoxAsync(Localizer, Localizer.PayrunResult.PayrunResults, exception);
            }
            else
            {
                await UserNotification.ShowErrorAsync(exception, exception.GetBaseMessage());
            }
        }
    }

    /// <summary>
    /// Payrun changed handler
    /// </summary>
    /// <param name="payrunName">The new payrun</param>
    private async Task PayrunChanged(string payrunName)
    {
        if (string.Equals(payrunName, SelectedPayrunName))
        {
            return;
        }
        var payrun = Payruns?.FirstOrDefault(x => string.Equals(x.Name, payrunName));
        if (payrun == null)
        {
            await UserNotification.ShowErrorAsync($"Unknown payrun {payrunName}");
            return;
        }

        // payrun selection
        SelectedPayrun = payrun;

        // refresh payrun job results grid
        if (Initialized)
        {
            await RefreshServerDataAsync();
        }
    }

    #endregion

    #region Payrun Jobs

    private void NavigateToJobs()
    {
        if (SelectedPayrun == null)
        {
            return;
        }
        var jobsUrl = $"{PageUrls.PayrunJobs}/{SelectedPayrun.Name}";
        NavigateTo(jobsUrl);
    }

    #endregion

    #region Grid

    private MudDataGrid<PayrollResultValue> ResultsGrid { get; set; }

    /// <summary>
    /// The grid column configuration
    /// </summary>
    private List<GridColumnConfiguration> ColumnConfiguration =>
        GetColumnConfiguration(GetTenantGridId(GridIdentifiers.PayrunResults));

    /// <summary>
    /// Dense mode
    /// <remarks>Based on the grid groups, dense is activated by default</remarks>
    /// </summary>
    private bool Dense { get; set; } = true;

    /// <summary>
    /// Toggle the grid dense state
    /// </summary>
    private async Task ToggleGridDenseAsync()
    {
        Dense = !Dense;

        // store dense mode
        await LocalStorage.SetItemAsBooleanAsync("PayrunResultDenseMode", Dense);
    }

    /// <summary>
    /// Reset all grid filters
    /// </summary>
    private async Task ResetFilterAsync() =>
        await ResultsGrid.ClearFiltersAsync();

    /// <summary>
    /// Download excel file from unfiltered grid data
    /// <remarks>Copy from <see cref="ItemPageBase{TItem,TQuery}.ExcelDownloadAsync"/> </remarks>
    /// </summary>
    private async Task ExcelDownloadAsync()
    {
        // server request
        var maxExport = Configuration.GetConfiguration<AppConfiguration>().ExcelExportMaxRecords;
        var state = ResultsGrid.BuildExportState(pageSize: maxExport);

        // retrieve all items, without any filter and sort
        var data = await GetServerDataAsync(state);
        var items = data.Items.ToList();
        if (!items.Any())
        {
            await UserNotification.ShowErrorMessageBoxAsync(Localizer, Localizer.PayrunResult.PayrunResults, Localizer.Error.EmptyCollection);
            return;
        }

        try
        {
            await ExcelDownload.StartAsync(ResultsGrid, items, JsRuntime, Localizer.PayrunResult.PayrunResults);
            await UserNotification.ShowSuccessAsync(Localizer.Shared.DownloadCompleted);
        }
        catch (Exception exception)
        {
            Log.Error(exception, exception.GetBaseMessage());
            await UserNotification.ShowErrorMessageBoxAsync(Localizer, Localizer.PayrunResult.PayrunResults, exception);
        }
    }

    /// <summary>
    /// Setup the grid
    /// </summary>
    private async Task SetupGrid()
    {
        var denseMode = await LocalStorage.GetItemAsBooleanAsync("PayrunResultDenseMode");
        if (denseMode.HasValue)
        {
            Dense = denseMode.Value;
        }
    }

    #endregion

    #region Payrun Results

    /// <summary>
    /// Get forecast payrun server data, handler for data grids
    /// </summary>
    /// <param name="state">The data grid state</param>
    /// <returns>Collection of items</returns>
    private async Task<GridData<PayrollResultValue>> GetServerDataAsync(GridState<PayrollResultValue> state)
    {
        try
        {
            if (SelectedPayrun == null)
            {
                throw new InvalidOperationException("Please ensure selected payrun");
            }

            // server request parameters
            Dictionary<string, object> parameters = new() {
                { nameof(PayrunJob.PayrunId), SelectedPayrun.Id }
            };
            return await PayrollResultBackendService.QueryAsync(state, parameters: parameters);
        }
        catch (Exception exception)
        {
            Log.Error(exception, exception.GetBaseMessage());
            return new();
        }
    }

    /// <summary>
    /// Refresh the forecast payrun jobs
    /// </summary>
    /// <returns></returns>
    private async Task RefreshServerDataAsync()
    {
        if (ResultsGrid == null)
        {
            return;
        }
        await ResultsGrid.ReloadServerData();
    }

    #endregion

    #region Lifecycle

    /// <summary>
    /// Setup page data after a tenant or payroll change
    /// </summary>
    private async Task SetupPage()
    {
        if (!HasPayroll)
        {
            return;
        }
        await SetupPayrunsAsync();
    }

    private bool Initialized { get; set; }
    protected override async Task OnInitializedAsync()
    {
        await SetupGrid();
        await SetupPage();
        await base.OnInitializedAsync();
        Initialized = true;
    }

    #endregion

}