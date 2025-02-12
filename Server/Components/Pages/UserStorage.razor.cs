using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using PayrollEngine.WebApp.Presentation;
using PayrollEngine.WebApp.Presentation.Component;

namespace PayrollEngine.WebApp.Server.Components.Pages;

public class UserStoragePageBase() : PageBase(WorkingItems.None)
{
    private static readonly string[] ExcludedItems =
    [
        "i18nextLng"
    ];

    #region Storage Item

    protected class StorageItem
    {
        public string Key { get; init; }
        public string Value { get; init; }
        public bool IsChecked { get; set; }

        public override string ToString() =>
            $"{Key}={Value}";
    }

    #endregion

    // local storage service
    [Inject]
    private ILocalStorageService LocalStorage { get; set; }

    /// <summary>
    /// Storage items
    /// </summary>
    protected ObservableCollection<StorageItem> Items { get; } = [];

    /// <summary>
    /// Delete storage item
    /// </summary>
    /// <param name="item">The item to remove</param>
    protected async Task DeleteAsync(StorageItem item)
    {
        // confirmation
        if (!await DialogService.ShowDeleteMessageBoxAsync(
                Localizer,
                Localizer.Item.DeleteTitle(Localizer.Storage.StorageItem),
                Localizer.Item.DeleteQuery(item.Key)))
        {
            return;
        }

        try
        {
            if (await LocalStorage.ContainKeyAsync(item.Key))
            {
                await LocalStorage.RemoveItemAsync(item.Key);
            }
            await UserNotification.ShowSuccessAsync(Localizer.Item.Deleted(item.Key));
        }
        catch (Exception exception)
        {
            LogError(exception);
            await UserNotification.ShowErrorMessageBoxAsync(Localizer, 
                Localizer.Item.DeleteTitle(Localizer.Storage.StorageItem), exception);
        }
        finally
        {
            await LoadAsync();
        }
    }

    /// <summary>
    /// Remove all user storage values
    /// </summary>
    protected async Task RemoveAllAsync()
    {
        if (Items.Count == 0)
        {
            return;
        }

        // confirmation
        if (!await DialogService.ShowDeleteMessageBoxAsync(
                Localizer,
                Localizer.Storage.ClearStorage,
                Localizer.Storage.ClearQuery(Items.Count)))
        {
            return;
        }

        try
        {
            await LocalStorage.ClearAsync();
            await UserNotification.ShowSuccessAsync(Localizer.Storage.Cleared);
        }
        catch (Exception exception)
        {
            LogError(exception);
            await UserNotification.ShowErrorMessageBoxAsync(Localizer, Localizer.Storage.ClearStorage, exception);
        }
        finally
        {
            await LoadAsync();
        }
    }

    /// <summary>
    /// Load storage items
    /// </summary>
    private async Task LoadAsync()
    {
        try
        {
            Items.Clear();

            // retrieve storage items
            var count = await LocalStorage.LengthAsync();
            var items = new List<StorageItem>();
            for (var i = 0; i < count; i++)
            {
                // storage key
                var key = await LocalStorage.KeyAsync(i);
                if (string.IsNullOrWhiteSpace(key))
                {
                    continue;
                }

                // excluded item
                if (ExcludedItems.Any(x => string.Equals(x, key, StringComparison.InvariantCultureIgnoreCase)))
                {
                    continue;
                }

                // storage value
                var value = await LocalStorage.GetItemAsStringAsync(key) ?? string.Empty;
                if (value.Length > 100)
                {
                    value = value.Substring(0, 100) + "...";
                }

                // storage item
                items.Add(new()
                {
                    Key = key,
                    Value = value
                });
            }

            // sort by key
            foreach (var item in items.OrderBy(x => x.Key))
            {
                Items.Add(item);
            }
        }
        catch (Exception exception)
        {
            LogError(exception);
            await UserNotification.ShowErrorMessageBoxAsync(Localizer, Localizer.Storage.Storage, exception);
        }
    }

    #region Lifecycle

    protected override async Task OnPageAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await LoadAsync();
            StateHasChanged();
        }
        await base.OnPageAfterRenderAsync(firstRender);
    }

    #endregion

}