using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using Microsoft.JSInterop;
using MudBlazor;
using PayrollEngine.Client;
using PayrollEngine.Client.Model;
using Task = System.Threading.Tasks.Task;

namespace PayrollEngine.WebApp.Presentation.Component;

public abstract class ItemPageBase<TItem, TQuery> : PageBase, IQueryResolver, IItemPageActions
    where TItem : class, IModel, IEquatable<TItem>, new()
    where TQuery : Query, new()
{
    [Inject]
    private IConfiguration Configuration { get; set; }
    [Inject]
    private IJSRuntime JsRuntime { get; set; }

    protected ItemPageBase(WorkingItems workingItems) :
        base(workingItems)
    {
    }

    #region Grid

    protected abstract string GridId { get; }
    protected abstract IBackendService<TItem, TQuery> BackendService { get; }
    protected abstract ItemCollection<TItem> Items { get; }

    protected MudDataGrid<TItem> ItemsGrid { get; set; }

    protected List<GridColumnConfiguration> ColumnConfiguration =>
        GetColumnConfiguration(GridId);

    public virtual string GetSortColumn(string expression)
    {
        if (string.IsNullOrWhiteSpace(expression))
        {
            return null;
        }

        // TODO identify sort column
        // this works only with one custom column type
        var column = ItemsGrid.RenderedColumns.FirstOrDefault(x => string.Equals(x.PropertyName.ToString(), expression));
        return GetColumnName(column);
    }

    public virtual string GetColumnName<T>(Column<T> column) where T : class
    {
        if (column == null)
        {
            return null;
        }
        // attribute column
        if (column.Tag is string attributeField)
        {
            return attributeField;
        }
        return column.PropertyName;
    }

    #endregion

    #region Backend services

    // refresh data grid with InvokeAsync (page reload errors)
    protected async Task RefreshServerDataAsync()
    {
        if (ItemsGrid == null)
        {
            return;
        }
        await ItemsGrid.ReloadServerData();
    }

    /// <summary>
    /// Get server data, handler for data grids
    /// </summary>
    /// <param name="state">The data grid state</param>
    /// <returns>Collection of items</returns>
    protected async Task<GridData<TItem>> GetServerDataAsync(GridState<TItem> state) =>
        await GetServerDataAsync(state, null);

    /// <summary>
    /// Get server data, handler for data grids
    /// </summary>
    /// <param name="state">The data grid state</param>
    /// <param name="parameters">The data request parameters</param>
    /// <returns>Collection of items</returns>
    private async Task<GridData<TItem>> GetServerDataAsync(GridState<TItem> state,
        Dictionary<string, object> parameters)
    {
        try
        {
            // server request parameters
            return await BackendService.QueryAsync(state, this, parameters);
        }
        catch (Exception exception)
        {
            Log.Error(exception, exception.GetBaseMessage());
            return new();
        }
    }

    /// <summary>
    /// Reset all grid filters
    /// </summary>
    public virtual async Task ResetFilterAsync() =>
        await ItemsGrid.ClearFiltersAsync();

    /// <summary>
    /// Download excel file from unfiltered grid data
    /// </summary>
    public virtual async Task ExcelDownloadAsync()
    {
        // server request
        var maxExport = Configuration.GetConfiguration<AppConfiguration>().ExcelExportMaxRecords;
        var state = ItemsGrid.BuildExportState(pageSize: maxExport);

        // retrieve all items, without any filter and sort
        var data = await GetServerDataAsync(state);
        var items = data.Items.ToList();
        if (!items.Any())
        {
            await UserNotification.ShowErrorMessageBoxAsync(Localizer,
                Localizer.Shared.ExcelDownload,
                Localizer.Error.EmptyCollection);
            return;
        }

        try
        {
            await ExcelDownload.StartAsync(ItemsGrid, items, JsRuntime);
            await UserNotification.ShowSuccessAsync(Localizer.Shared.DownloadCompleted);
        }
        catch (Exception exception)
        {
            Log.Error(exception, exception.GetBaseMessage());
            await UserNotification.ShowErrorMessageBoxAsync(Localizer, Localizer.Error.FileDownloadError, exception);
        }
    }

    protected async Task<TItem> GetItemAsync(int itemId)
    {
        var item = Items.FirstOrDefault(x => x.Id == itemId);
        if (item != null)
        {
            return item;
        }

        // inactive item
        try
        {
            item = await BackendService.GetAsync(itemId);
        }
        catch (Exception exception)
        {
            Log.Error(exception, exception.GetBaseMessage());
            return new();
        }
        return item;
    }

    #endregion

    #region Working Items

    protected override async Task OnTenantChangedAsync()
    {
        if (WorkingItems.TenantAvailable())
        {
            // refresh custom columns, stored by tenant
            SetupCustomColumns(GridId);
            await RefreshServerDataAsync();
        }

        await base.OnTenantChangedAsync();
    }

    protected override async Task OnEmployeeChangedAsync(Employee employee)
    {
        if (WorkingItems.EmployeeAvailable())
        {
            await RefreshServerDataAsync();
        }
        await base.OnEmployeeChangedAsync(employee);
    }

    protected override async Task OnPayrollChangedAsync(Payroll payroll)
    {
        if (WorkingItems.PayrollAvailable())
        {
            await RefreshServerDataAsync();
        }
        await base.OnPayrollChangedAsync(payroll);
    }

    #endregion

    protected override async Task OnInitializedAsync()
    {
        SetupCustomColumns(GridId);
        await base.OnInitializedAsync();
    }
}