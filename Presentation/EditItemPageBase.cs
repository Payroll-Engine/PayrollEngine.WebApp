using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using PayrollEngine.Client;
using PayrollEngine.Client.Model;
using Task = System.Threading.Tasks.Task;

namespace PayrollEngine.WebApp.Presentation;

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

    protected static string ItemTypeUiName =>
        ItemTypeName.ToPascalSentence();

    public virtual async Task AddItemAsync()
    {
        // dialog options
        var options = new DialogOptions();
        if (!await SetupDialogOptionsAsync(options, ItemOperation.Create))
        {
            return;
        }

        // dialog parameters
        var parameters = new DialogParameters();
        if (!await SetupDialogParametersAsync(parameters, ItemOperation.Create))
        {
            return;
        }
        if (IsDialogParameter(nameof(Tenant)))
        {
            parameters.Add(nameof(Tenant), Tenant);
        }

        // dialog
        var dialog = await (await DialogService.ShowAsync<TDialog>($"Create {ItemTypeUiName}", parameters, options)).Result;
        if (dialog == null || dialog.Canceled)
        {
            return;
        }

        // validation
        var item = dialog.Data as TItem;
        if (item == null || !await OnItemCommit(item, ItemOperation.Create))
        {
            return;
        }

        // create
        try
        {
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
            await OnItemCompleteAsync(createdItem, ItemOperation.Create);
            await UserNotification.ShowInformationAsync($"{ItemTypeUiName} successfully created");
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

        // existing
        var existing = await GetItemAsync(item.Id);
        if (existing == null)
        {
            await UserNotification.ShowErrorAsync($"Unknown {ItemTypeUiName} {item} to update");
            return;
        }

        // dialog options
        var options = new DialogOptions();
        if (!await SetupDialogOptionsAsync(options, ItemOperation.Edit))
        {
            return;
        }

        // dialog parameters
        var parameters = new DialogParameters {
        {
            ItemTypeName, item
        } };
        if (!await SetupDialogParametersAsync(parameters, ItemOperation.Edit))
        {
            return;
        }
        if (IsDialogParameter(nameof(Tenant)))
        {
            parameters.Add(nameof(Tenant), Tenant);
        }

        // dialog
        var dialog = await (await DialogService.ShowAsync<TDialog>($"Edit {ItemTypeUiName}", parameters, options)).Result;
        if (dialog == null || dialog.Canceled)
        {
            return;
        }

        // validation
        var updatedItem = dialog.Data as TItem;
        if (updatedItem == null || !await OnItemCommit(updatedItem, ItemOperation.Edit))
        {
            return;
        }

        // update
        try
        {
            await BackendService.UpdateAsync(updatedItem);

            // notification
            await OnItemCompleteAsync(updatedItem, ItemOperation.Edit);
            await UserNotification.ShowInformationAsync($"{ItemTypeUiName} successfully updated");
        }
        catch (Exception exception)
        {
            Log.Error(exception, exception.GetBaseMessage());
            await UserNotification.ShowErrorAsync(exception);
            return;
        }

        await RefreshServerDataAsync();
    }

    private bool IsDialogParameter(string name) =>
        typeof(TDialog).GetProperty(name) != null;

    public async Task DeleteItemAsync(TItem item)
    {
        if (item == null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        // existing
        var deleteItem = await GetItemAsync(item.Id);
        if (deleteItem == null)
        {
            await UserNotification.ShowErrorAsync($"Unknown {ItemTypeUiName} {item} to delete");
            return;
        }

        // confirmation
        if (!await DialogService.ShowDeleteMessageBoxAsync(
                $"Delete {ItemTypeUiName}",
                new MarkupString($"Delete <b>&#xbb;{item.GetUiString()}&#xab;</b> permanently?")))
        {
            return;
        }

        // validation
        if (!await OnItemCommit(deleteItem, ItemOperation.Delete))
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
            await OnItemCompleteAsync(item, ItemOperation.Delete);
            await UserNotification.ShowInformationAsync($"{ItemTypeUiName} successfully deleted");
        }
        catch (Exception exception)
        {
            Log.Error(exception, exception.GetBaseMessage());
            await UserNotification.ShowErrorAsync(exception);
            return;
        }

        await RefreshServerDataAsync();
    }

    protected virtual Task<bool> SetupDialogOptionsAsync(DialogOptions options, ItemOperation operation) =>
        Task.FromResult(true);

    protected virtual Task<bool> SetupDialogParametersAsync(DialogParameters parameters, ItemOperation operation) =>
        Task.FromResult(true);

    protected virtual Task<bool> OnItemCommit(TItem payroll, ItemOperation operation) =>
        Task.FromResult(true);

    protected virtual Task OnItemChanged(TItem item)
    {
        return Task.CompletedTask;
    }

    protected virtual Task OnItemCompleteAsync(TItem item, ItemOperation operation)
    {
        return Task.CompletedTask;
    }

    #endregion
}