using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Blazored.LocalStorage;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using NPOI.XSSF.UserModel;
using MudBlazor;
using PayrollEngine.IO;
using PayrollEngine.Client.Service;
using PayrollEngine.Client.QueryExpression;
using PayrollEngine.WebApp.ViewModel;
using PayrollEngine.WebApp.Presentation.BackendService;
using PayrollEngine.WebApp.Server.Shared;
using PayrollEngine.Data;
using PayrollEngine.Document;
using Task = System.Threading.Tasks.Task;
using Microsoft.Extensions.Configuration;
using PayrollEngine.WebApp.Presentation;

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
        base(WorkingItems.TenantChange | WorkingItems.PayrollChange)
    {
    }

    /// <inheritdoc />
    protected override async Task OnTenantChangedAsync(Client.Model.Tenant tenant)
    {
        await SetupPage();
        await base.OnTenantChangedAsync(tenant);
        StateHasChanged();
    }

    /// <inheritdoc />
    protected override async Task OnPayrollChangedAsync(Client.Model.Payroll payroll)
    {
        await SetupPage();
        await base.OnPayrollChangedAsync(payroll);
        StateHasChanged();
    }

    #region Payruns

    /// <summary>
    /// The payruns of the working payroll
    /// </summary>
    protected List<Payrun> Payruns { get; set; }

    /// <summary>
    /// True if payroll contains payruns
    /// </summary>
    protected bool HasPayruns => Payruns != null && Payruns.Any();

    /// <summary>
    /// Selected payrun
    /// </summary>
    protected Payrun SelectedPayrun { get; set; }

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
            List<Payrun> payruns =
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
                await UserNotification.ShowErrorMessageBoxAsync("Payruns setup", exception);
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

    #region Grid

    private MudDataGrid<PayrollResultValue> ResultsGrid { get; set; }

    /// <summary>
    /// The grid column configuration
    /// </summary>
    protected List<GridColumnConfiguration> ColumnConfiguration =>
        GetColumnConfiguration(GetTenantGridId(GridIdentifiers.PayrunResults));

    /// <summary>
    /// Dense mode
    /// <remarks>Based on the grid groups, dense is activated by default</remarks>
    /// </summary>
    protected bool Dense { get; set; } = true;

    /// <summary>
    /// Toggle the grid dense state
    /// </summary>
    protected async Task ToggleGridDenseAsync()
    {
        Dense = !Dense;

        // store dense mode
        await LocalStorage.SetItemAsBooleanAsync("PayrunResultDenseMode", Dense);
    }

    /// <summary>
    /// Reset all grid filters
    /// </summary>
    protected async Task ResetFilterAsync() =>
        await ResultsGrid.ClearFiltersAsync();


    /// <summary>
    /// Reset all grid filters
    /// </summary>
    protected async Task ExcelDownloadAsync()
    {
        // server request
        var maxExport = Configuration.GetConfiguration<AppConfiguration>().ExcelExportMaxRecords;
        var state = ResultsGrid.BuildExportState(pageSize: maxExport);

        // retrieve all items, without any filter and sort
        var data = await GetServerDataAsync(state);
        var items = data.Items.ToList();
        if (!items.Any())
        {
            await UserNotification.ShowErrorMessageBoxAsync("Excel Download", "Empty collection");
            return;
        }

        try
        {
            var name = "PayrunResults";
            // convert items to data set
            var dataSet = new System.Data.DataSet(name);

            var properties = ResultsGrid.RenderedColumns.Select(x => x.PropertyName).ToList();
            var dataTable = items.ToSystemDataTable(name, includeRows: true, properties: properties);
            dataSet.Tables.Add(dataTable);

            // xlsx workbook
            using var workbook = new XSSFWorkbook();
            // import 
            workbook.Import(dataSet);

            // result
            using var resultStream = new MemoryStream();
            workbook.Write(resultStream, true);
            resultStream.Seek(0, SeekOrigin.Begin);

            var download = $"{name}_{FileTool.CurrentTimeStamp()}{FileExtensions.ExcelDocument}";
            await JsRuntime.SaveAs(download, resultStream.ToArray());
            await UserNotification.ShowSuccessAsync("Download completed");
        }
        catch (Exception exception)
        {
            await UserNotification.ShowErrorMessageBoxAsync("Excel download error", exception);
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
    protected async Task<GridData<PayrollResultValue>> GetServerDataAsync(GridState<PayrollResultValue> state)
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
    protected async Task RefreshServerDataAsync()
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
    protected async Task SetupPage()
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