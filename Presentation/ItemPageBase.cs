using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MudBlazor;
using PayrollEngine.Client;
using PayrollEngine.Client.Model;
using Task = System.Threading.Tasks.Task;

namespace PayrollEngine.WebApp.Presentation;

public abstract class ItemPageBase<TItem, TQuery> : PageBase, IQueryResolver
    where TItem : class, IModel, IEquatable<TItem>, new()
    where TQuery : Query, new()
{
    protected ItemPageBase(WorkingItems workingItems) :
        base(workingItems)
    {
    }

    #region Data and Grid

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
    protected virtual async Task<GridData<TItem>> GetServerDataAsync(GridState<TItem> state,
        Dictionary<string, object> parameters)
    {
        try
        {
            // server request parameters
            parameters ??= new();
            if (!await PrepareServerDataRequestAsync(state, parameters))
            {
                return new();
            }

            return await BackendService.QueryAsync(state, this, parameters);
        }
        catch (Exception exception)
        {
            Log.Error(exception, exception.GetBaseMessage());
            return new();
        }
    }

    /// <summary>
    /// Setup the server request
    /// </summary>
    /// <param name="state">The data grid state</param>
    /// <param name="parameters">The data request parameters</param>
    /// <returns>True for a valid request, otherwise the request will be return an empty collection</returns>
    protected virtual Task<bool> PrepareServerDataRequestAsync(
        GridState<TItem> state, IDictionary<string, object> parameters) =>
        Task.FromResult(true);

    protected virtual async Task<TItem> GetItemAsync(int itemId)
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

    protected override async Task OnTenantChangedAsync(Tenant tenant)
    {
        if (WorkingItems.TenantAvailable())
        {
            // refresh custom columns, stored by tenant
            SetupCustomColumns(GridId);
            await RefreshServerDataAsync();
        }

        await base.OnTenantChangedAsync(tenant);
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