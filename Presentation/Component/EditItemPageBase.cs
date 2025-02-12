using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using PayrollEngine.Client;
using PayrollEngine.Client.Model;
using PayrollEngine.Client.Service;
using Task = System.Threading.Tasks.Task;

namespace PayrollEngine.WebApp.Presentation.Component;

public abstract class EditItemPageBase<TItem, TQuery, TDialog>(WorkingItems workingItems,
    bool useCalendar = false) :
    ItemPageBase<TItem, TQuery>(workingItems), IItemOperator<TItem>
        where TItem : class, IModel, IKeyEquatable<TItem>, new()
        where TQuery : Query, new()
        where TDialog : ComponentBase
{
    [Inject]
    private ICalendarService CalendarService { get; set; }

    private bool UseCalendar { get; } = useCalendar;

    #region Model Operations

    protected static string ItemTypeName =>
        typeof(TItem).Name;

    protected string ItemTypeUiName =>
        Localizer.GroupKey(ItemTypeName);

    protected virtual bool AddItemTenantParameter => true;

    /// <summary>
    /// Get the tenant of the edit item, by default is this the working tenant
    /// </summary>
    /// <remarks>This method should be overriden while editing tenants</remarks>
    protected virtual Tenant GetItemTenant(ItemOperation operation, TItem item) => Tenant;

    public virtual async Task CreateItemAsync()
    {
        // dialog parameters
        var parameters = new DialogParameters();
        if (!await SetupDialogParametersAsync(parameters, ItemOperation.Create, null))
        {
            return;
        }

        // create
        try
        {
            // item tenant
            var itemTenant = GetItemTenant(ItemOperation.Create, null);

            // ensure tenant parameter
            if (AddItemTenantParameter && IsDialogParameter(nameof(Tenant)))
            {
                var tenant = parameters.TryGet<Tenant>(nameof(Tenant));
                if (tenant == null)
                {
                    parameters.Add(nameof(Tenant), itemTenant);
                }
            }

            // ensure culture parameter
            if (IsDialogParameter(nameof(Tenant.Culture)))
            {
                var culture = parameters.TryGet<string>(nameof(Tenant.Culture));
                if (culture == null)
                {
                    parameters.Add(nameof(Tenant.Culture), GetTenantCulture(itemTenant));
                }
            }

            // ensure calendar names parameter
            if (UseCalendar)
            {
                var calendars = await CalendarService.QueryAsync<Calendar>(new(itemTenant.Id));
                var calendarNames = calendars.Select(x => x.Name).ToList();
                parameters.Add(nameof(calendarNames), calendarNames);
            }

            // dialog
            var dialog = await (await DialogService.ShowAsync<TDialog>(
                title: Localizer.Item.CreateTitle(ItemTypeUiName),
                parameters: parameters)).Result;
            if (dialog == null || dialog.Canceled)
            {
                return;
            }

            // validation
            var item = dialog.Data as TItem;
            if (item == null || !await OnItemCommit(item))
            {
                return;
            }

            item = await BackendService.CreateAsync(item);
            if (item == null || item.Id == 0)
            {
                return;
            }

            var createdItem = await BackendService.GetAsync(item.Id);
            if (createdItem == null)
            {
                return;
            }

            // session
            Items.Add(createdItem);

            // notification
            await UserNotification.ShowInformationAsync(Localizer.Item.Created(ItemTypeUiName));
        }
        catch (Exception exception)
        {
            Log.Error(exception, exception.GetBaseMessage());
            await UserNotification.ShowErrorAsync(exception);
            return;
        }

        await RefreshServerDataAsync();
    }

    public virtual async Task EditItemAsync(TItem item)
    {
        if (item == null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        // update
        try
        {
            // existing
            var existing = await GetItemAsync(item.Id);
            if (existing == null)
            {
                await UserNotification.ShowErrorAsync(Localizer.Error.UnknownItem(ItemTypeUiName, item));
                return;
            }

            // item tenant
            var itemTenant = GetItemTenant(ItemOperation.Edit, item);

            // dialog parameters
            var parameters = new DialogParameters {
            {
                ItemTypeName, item
            } };
            if (!await SetupDialogParametersAsync(parameters, ItemOperation.Edit, item))
            {
                return;
            }

            // ensure tenant parameter
            if (IsDialogParameter(nameof(Tenant)))
            {
                var tenant = parameters.TryGet<Tenant>(nameof(Tenant));
                if (tenant == null)
                {
                    parameters.Add(nameof(Tenant), itemTenant);
                }
            }

            // ensure culture parameter
            if (IsDialogParameter(nameof(Tenant.Culture)))
            {
                var culture = parameters.TryGet<string>(nameof(Tenant.Culture));
                if (culture == null)
                {
                    parameters.Add(nameof(Tenant.Culture), GetTenantCulture(itemTenant));
                }
            }

            // ensure calendar names parameter
            if (UseCalendar)
            {
                var calendars = await CalendarService.QueryAsync<Calendar>(new(itemTenant.Id));
                var calendarNames = calendars.Select(x => x.Name).ToList();
                parameters.Add(nameof(calendarNames), calendarNames);
            }

            // dialog
            var title = Localizer.Item.EditTitle(ItemTypeUiName);

            var dialog = await (await DialogService.ShowAsync<TDialog>(title, parameters)).Result;
            if (dialog == null || dialog.Canceled)
            {
                await RefreshServerDataAsync();
                return;
            }

            // validation
            var updatedItem = dialog.Data as TItem;
            if (updatedItem == null || !await OnItemCommit(updatedItem))
            {
                return;
            }

            await BackendService.UpdateAsync(updatedItem);

            // notification
            await UserNotification.ShowInformationAsync(Localizer.Item.Updated(ItemTypeUiName));
        }
        catch (Exception exception)
        {
            Log.Error(exception, exception.GetBaseMessage());
            await UserNotification.ShowErrorAsync(exception);
            return;
        }

        await RefreshServerDataAsync();
    }

    private static bool IsDialogParameter(string name) =>
        typeof(TDialog).GetProperty(name) != null;

    public virtual async Task DeleteItemAsync(TItem item)
    {
        if (item == null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        // existing
        var deleteItem = await GetItemAsync(item.Id);
        if (deleteItem == null)
        {
            await UserNotification.ShowErrorAsync(Localizer.Error.UnknownItem(ItemTypeUiName, item));
            return;
        }

        // confirmation
        if (!await DialogService.ShowDeleteMessageBoxAsync(
                localizer: Localizer,
                title: Localizer.Item.DeleteTitle(ItemTypeUiName),
                message: Localizer.Item.DeleteMarkupQuery(item.GetUiString())))
        {
            return;
        }

        // validation
        if (!await OnItemCommit(deleteItem))
        {
            return;
        }

        // delete
        if (!await BackendService.DeleteAsync(item.Id))
        {
            return;
        }

        // delete
        try
        {
            Items.Remove(item);

            // notification
            await UserNotification.ShowInformationAsync(Localizer.Item.Deleted(ItemTypeUiName));
        }
        catch (Exception exception)
        {
            Log.Error(exception, exception.GetBaseMessage());
            await UserNotification.ShowErrorAsync(exception);
            return;
        }

        await RefreshServerDataAsync();
    }

    /// <summary>
    /// Setup the dialog parameters
    /// </summary>
    /// <param name="parameters">The parameter collection</param>
    /// <param name="operation">The item operation</param>
    /// <param name="item">The edit item</param>
    /// <returns>True for valid dialog parameters</returns>
    protected virtual Task<bool> SetupDialogParametersAsync(DialogParameters parameters,
        ItemOperation operation, TItem item) =>
        Task.FromResult(true);

    /// <summary>
    /// Validate the Item before commit
    /// </summary>
    /// <param name="item">The item to validate</param>
    /// <returns>True for a valid item</returns>
    protected virtual Task<bool> OnItemCommit(TItem item) =>
        Task.FromResult(true);

    #endregion

}