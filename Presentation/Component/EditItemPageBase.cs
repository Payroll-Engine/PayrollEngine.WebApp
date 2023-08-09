﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using PayrollEngine.Client;
using PayrollEngine.Client.Model;
using Task = System.Threading.Tasks.Task;

namespace PayrollEngine.WebApp.Presentation.Component;

public abstract class EditItemPageBase<TItem, TQuery, TDialog> : ItemPageBase<TItem, TQuery>, IItemOperator<TItem>
    where TItem : class, IModel, IKeyEquatable<TItem>, new()
    where TQuery : Query, new()
    where TDialog : ComponentBase
{
    protected EditItemPageBase(WorkingItems workingItems) :
        base(workingItems)
    {
    }

    #region Model Operations

    protected static string ItemTypeName =>
        typeof(TItem).Name;

    protected string ItemTypeUiName =>
        Localizer.GroupKey(ItemTypeName);

    protected virtual bool AddItemTenantParameter => true;

    public virtual async Task AddItemAsync()
    {
        // dialog parameters
        var parameters = new DialogParameters();
        if (!await SetupDialogParametersAsync<TItem>(parameters, ItemOperation.Create, null))
        {
            return;
        }

        // create
        try
        {

            // ensure tenant parameter
            if (AddItemTenantParameter && IsDialogParameter(nameof(Tenant)))
            {
                var tenant = parameters.TryGet<Tenant>(nameof(Tenant));
                if (tenant == null)
                {
                    parameters.Add(nameof(Tenant), Tenant);
                }
            }

            // ensure culture parameter
            if (IsDialogParameter(nameof(Tenant.Culture)))
            {
                var culture = parameters.TryGet<string>(nameof(Tenant.Culture));
                if (culture == null)
                {
                    parameters.Add(nameof(Tenant.Culture), PageCulture);
                }
            }

            // dialog
            var dialog = await (await DialogService.ShowAsync<TDialog>(Localizer.Item.AddTitle(ItemTypeUiName), parameters)).Result;
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
            await UserNotification.ShowInformationAsync(Localizer.Item.Added(ItemTypeUiName));
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
                    parameters.Add(nameof(Tenant), Tenant);
                }
            }

            // ensure culture parameter
            if (IsDialogParameter(nameof(Tenant.Culture)))
            {
                var culture = parameters.TryGet<string>(nameof(Tenant.Culture));
                if (culture == null)
                {
                    parameters.Add(nameof(Tenant.Culture), PageCulture);
                }
            }

            // dialog
            var title = Localizer.Item.EditTitle(ItemTypeUiName);

            var dialog =  await (await DialogService.ShowAsync<TDialog>(title, parameters)).Result;
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
                Localizer,
                Localizer.Item.DeleteTitle(ItemTypeUiName),
                Localizer.Item.DeleteQuery(item.GetUiString())))
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
    protected virtual Task<bool> SetupDialogParametersAsync<T>(DialogParameters parameters, 
        ItemOperation operation, T item) =>
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