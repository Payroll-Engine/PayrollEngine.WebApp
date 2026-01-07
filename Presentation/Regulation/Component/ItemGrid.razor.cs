using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Specialized;
using Task = System.Threading.Tasks.Task;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Blazored.LocalStorage;
using PayrollEngine.WebApp.Shared;
using PayrollEngine.WebApp.ViewModel;
using PayrollEngine.WebApp.Presentation.Regulation.Factory;

namespace PayrollEngine.WebApp.Presentation.Regulation.Component;

public partial class ItemGrid<TParent, TItem> : ComponentBase, IDisposable
    where TParent : class, IRegulationItem
    where TItem : class, IRegulationItem
{
    [Parameter]
    public List<Client.Model.Regulation> Regulations { get; set; }
    [Parameter]
    public string KeyHeader { get; set; }
    [Parameter]
    public string ParentKeyHeader { get; set; }
    [Parameter]
    public bool ShowDescription { get; set; }
    [Parameter]
    public bool ShowAdditionalInfo { get; set; } = true;
    [Parameter]
    public string AdditionalInfoHeader { get; set; }
    [Parameter]
    public ItemCollection<TItem> Items { get; set; }
    [Parameter]
    public ItemGridConfig ItemGridConfig { get; set; }
    [Parameter]
    public IItemFactory<TParent> ParentFactory { get; set; }
    [Parameter]
    public RegulationItemType ItemType { get; set; }
    [Parameter]
    public bool ItemSelection { get; set; } = true;
    [Parameter]
    public EventCallback<TItem> SelectedItemChanged { get; set; }

    [Inject]
    private IThemeService ThemeService { get; set; }
    [Inject]
    private IDialogService DialogService { get; set; }
    [Inject]
    private ILocalStorageService LocalStorage { get; set; }
    [Inject]
    private IUserNotificationService NotificationService { get; set; }
    [Inject]
    private IJSRuntime JsRuntime { get; set; }
    [Inject]
    private ILocalizerService LocalizerService { get; set; }

    private Localizer Localizer => LocalizerService.Localizer;
    private bool HasParent => ParentFactory != null;
    private MudDataGrid<TItem> ItemsGrid { get; set; }

    private string GetParentTypeName()
    {
        if (!ItemType.HasParentType())
        {
            return null;
        }

        return Localizer.GroupKey(ItemType.ParentType().ToString());
    }

    private string GetItemTypeName(bool multiple = false)
    {
        var key = ItemType.ToString();
        return Localizer.Key(key, multiple ? $"{key}s" : key);
    }

    private int RowsPerPage => ItemGridConfig.ItemsPageSize;

    private bool Dense
    {
        get => ItemGridConfig.DenseMode;
        set => ItemGridConfig.DenseMode = value;
    }

    /// <summary>
    /// Toggle the grid dense state
    /// </summary>
    private async Task ToggleGridDenseAsync()
    {
        Dense = !Dense;

        // store dense mode
        await LocalStorage.SetItemAsBooleanAsync("RegulationDenseMode", Dense);
    }

    private async Task ExpandItemGroupsAsync() =>
        await ItemsGrid.ExpandAllGroupsAsync();

    private async Task CollapseItemGroupsAsync() =>
        await ItemsGrid.CollapseAllGroupsAsync();

    private async Task OnSelectedItemChangedAsync(TItem item) =>
        await SelectedItemChanged.InvokeAsync(item);

    private async Task AddItemAsync()
    {
        // regulation
        var regulation = Regulations?.FirstOrDefault();
        if (regulation == null)
        {
            return;
        }

        // new child item
        if (HasParent)
        {
            var parentItems = await ParentFactory.QueryPayrollItemsAsync();
            if (!parentItems.Any())
            {
                Log.Warning($"Missing parent {GetParentTypeName()}");
                return;
            }

            // dialog
            var parameters = new DialogParameters
            {
                { nameof(ParentItemDialog<>.Items), parentItems }
            };
            var dialog = await (await DialogService.ShowAsync<ParentItemDialog<TParent>>(
                Localizer.Item.SelectParent(GetParentTypeName()), parameters)).Result;
            if (dialog == null || dialog.Canceled)
            {
                return;
            }

            // parent
            var parent = parentItems.FirstOrDefault(x => string.Equals(x.Name, dialog.Data as string));
            if (parent == null)
            {
                Log.Warning($"Invalid parent {GetParentTypeName()}");
                return;
            }

            await CreateItemAsync(parent);
            return;
        }

        // object without parent
        await CreateItemAsync();
    }

    private async Task ExcelDownloadAsync()
    {
        try
        {
            await ExcelDownload.StartAsync(ItemsGrid, Items, JsRuntime, ItemType.LocalizedName(Localizer));
            await NotificationService.ShowSuccessAsync(Localizer.Shared.DownloadCompleted);
        }
        catch (Exception exception)
        {
            Log.Error(exception, exception.GetBaseMessage());
            await NotificationService.ShowErrorMessageBoxAsync(Localizer, Localizer.Error.FileDownloadError, exception);
        }
    }

    private async Task CreateItemAsync(TParent parent = null)
    {
        // regulation
        var regulation = Regulations?.FirstOrDefault();
        if (regulation == null)
        {
            return;
        }

        // create object
        var newObject = Activator.CreateInstance<TItem>();
        if (newObject == null)
        {
            return;
        }

        // object setup
        newObject.RegulationName = regulation.Name;
        newObject.Parent = parent;

        // notification
        await OnSelectedItemChangedAsync(newObject);

        // reset grid selection
        await ItemsGrid.SetSelectedItemAsync(newObject);
    }

    private TItem lastSelected;
    private async Task SelectedItemChangedAsync(TItem item)
    {
        await OnSelectedItemChangedAsync(item);
        lastSelected = item;
    }

    private string RowStyleHandler(TItem item, int _)
    {
        if (item == lastSelected)
        {
            return $"background-color: {ThemeService.SelectedBackgroundColor()};";
        }
        return string.Empty;
    }

    protected override async Task OnInitializedAsync()
    {
        Items.CollectionChanged += ItemCollectionChanged;
        await base.OnInitializedAsync();
    }

    private void ItemCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
    {
        if (args.Action == NotifyCollectionChangedAction.Add &&
            args.NewItems != null && args.NewItems.Count == 1)
        {
            var item = args.NewItems[0] as TItem;
            ItemsGrid.SetSelectedItemAsync(item).Wait();
        }
        else if (args.Action == NotifyCollectionChangedAction.Remove &&
                 ItemsGrid.SelectedItem != null &&
                 args.OldItems != null && args.OldItems.Contains(ItemsGrid.SelectedItem))
        {
            ItemsGrid.SetSelectedItemAsync(null).Wait();
        }
    }

    public void Dispose()
    {
        Items.CollectionChanged -= ItemCollectionChanged;
    }
}
