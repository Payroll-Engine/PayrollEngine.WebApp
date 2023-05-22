using System;
using System.Collections.Generic;
using System.Linq;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using PayrollEngine.WebApp.Presentation.Regulation.Factory;
using PayrollEngine.WebApp.ViewModel;
using Task = System.Threading.Tasks.Task;

namespace PayrollEngine.WebApp.Presentation.Regulation.Component;

public partial class ItemGrid<TParent, TItem> : ComponentBase
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
    public int ItemsPageSize { get; set; } = 20;
    [Parameter] public IItemFactory<TParent> ParentFactory { get; set; } = null;
    [Parameter]
    public RegulationItemType ItemType { get; set; }
    [Parameter]
    public EventCallback<TItem> SelectedItemChanged { get; set; }

    [Inject]
    public IThemeService ThemeService { get; set; }
    [Inject]
    private IDialogService DialogService { get; set; }
    [Inject]
    public ILocalStorageService LocalStorage { get; set; }

    protected bool HasParent => ParentFactory != null;
    protected MudDataGrid<TItem> ItemsGrid { get; set; }

    protected string GetParentTypeName()
    {
        if (!ItemType.HasParentType())
        {
            return null;
        }

        return ItemType.ParentType().ToString().ToPascalSentence();
    }

    protected string GetItemTypeName(bool multiple = false)
    {
        var itemTypeName = ItemType.ToString().ToPascalSentence();
        return multiple ? $"{itemTypeName}s" : itemTypeName;
    }

    protected bool Dense { get; set; }

    /// <summary>
    /// Toggle the grid dense state
    /// </summary>
    protected async Task ToggleGridDenseAsync()
    {
        Dense = !Dense;

        // store dense mode
        await LocalStorage.SetItemAsBooleanAsync("RegulationDenseMode", Dense);
    }

    /// <summary>
    /// Setup the grid
    /// </summary>
    private async Task SetupGridAsync()
    {
        var denseMode = await LocalStorage.GetItemAsBooleanAsync("RegulationDenseMode");
        if (denseMode.HasValue)
        {
            Dense = denseMode.Value;
        }
    }

    protected void ExpandItemGroups() =>
        ItemsGrid.ExpandAllGroups();

    protected void CollapseItemGroups() =>
        ItemsGrid.CollapseAllGroups();

    private async Task OnSelectedItemChanged(TItem item) =>
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
            var parentItems = await ParentFactory.QueryPayrollItems();
            if (!parentItems.Any())
            {
                Log.Warning($"Missing parent {typeof(TParent).Name.ToPascalSentence()}");
                return;
            }

            // dialog
            var parameters = new DialogParameters
            {
                { nameof(ParentItemDialog<TItem>.Items), parentItems }
            };
            var dialog = await (await DialogService.ShowAsync<ParentItemDialog<TParent>>($"Select {GetParentTypeName()}", parameters)).Result;
            if (dialog == null || dialog.Canceled)
            {
                return;
            }

            // parent
            var parent = parentItems.FirstOrDefault(x => string.Equals(x.Name, dialog.Data as string));
            if (parent == null)
            {
                Log.Warning($"Invalid parent {typeof(TParent).Name.ToPascalSentence()}");
                return;
            }

            await CreateItemAsync(parent);
            return;
        }

        // object without parent
        await CreateItemAsync();
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
        if (newObject != null)
        {
            // object setup
            newObject.RegulationName = regulation.Name;
            newObject.RegulationId = regulation.Id;
            newObject.Parent = parent;

            // notification
            await OnSelectedItemChanged(newObject);
        }
    }

    protected async Task SelectedItemChangedAsync(TItem item) =>
        await OnSelectedItemChanged(item);

    protected string RowStyleHandler(TItem item, int _)
    {
        var selectedItem = ItemsGrid.SelectedItem;
        if (selectedItem != null && item.Id == selectedItem.Id)
        {
            var color = ThemeService.SelectedWorkingTypeColor();
            return $"background-color: {color}";
        }
        return string.Empty;
    }

    protected override async Task OnInitializedAsync()
    {
        await SetupGridAsync();
        await base.OnInitializedAsync();
    }
}
