using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using PayrollEngine.WebApp.Presentation;

namespace PayrollEngine.WebApp.Server.Pages;

public class UserStoragePageBase : PageBase
{
    private static readonly string[] ExcludedItems = {
        "i18nextLng"
    };

    #region Storage Item

    public class StorageItem
    {
        public string Key { get; set; }
        public string Value { get; set; }
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
    protected ObservableCollection<StorageItem> Items { get; } = new();

    public UserStoragePageBase() :
        base(WorkingItems.None)
    {
    }

    /// <summary>
    /// Remove storage item
    /// </summary>
    /// <param name="item">The item to remove</param>
    protected async Task RemoveAsync(StorageItem item)
    {
        // confirmation
        if (!await DialogService.ShowDeleteMessageBoxAsync(
                "Remove storage item",
                new MarkupString($"Remove storage item <b>&#xbb;{item.Key}&#xab;</b>?")))
        {
            return;
        }

        try
        {
            if (await LocalStorage.ContainKeyAsync(item.Key))
            {
                await LocalStorage.RemoveItemAsync(item.Key);
            }
            await UserNotification.ShowSuccessAsync($"Removed {item.Key}");
        }
        catch (Exception exception)
        {
            LogError(exception);
            await UserNotification.ShowErrorMessageBoxAsync("Remove storage item", exception);
        }
        finally
        {
            await LoadAsync();
        }
    }

    /// <summary>
    /// Remove all user storage values
    /// </summary>
    /// <returns></returns>
    protected async Task RemoveAllAsync()
    {
        if (Items.Count == 0)
        {
            return;
        }

        // confirmation
        if (!await DialogService.ShowDeleteMessageBoxAsync(
                "Clear user storage", new MarkupString($"Remove all {Items.Count} storage items?")))
        {
            return;
        }

        try
        {
            await LocalStorage.ClearAsync();
            await UserNotification.ShowSuccessAsync("Storage cleared");
        }
        catch (Exception exception)
        {
            LogError(exception);
            await UserNotification.ShowErrorMessageBoxAsync("Clear user storage", exception);
        }
        finally
        {
            await LoadAsync();
        }
    }

    /// <summary>
    /// Load storage items
    /// </summary>
    protected async Task LoadAsync()
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
            await UserNotification.ShowErrorMessageBoxAsync("Load user storage", exception);
        }
    }

    protected override async Task OnInitializedAsync()
    {
        await LoadAsync();
        await base.OnInitializedAsync();
    }
}