using System;
using System.Linq;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using PayrollEngine.WebApp.Shared;
using Task = System.Threading.Tasks.Task;

namespace PayrollEngine.WebApp.Presentation.Component;

public partial class AttributesGrid : IDisposable
{
    [Parameter]
    public IAttributeObject Item { get; set; }
    [Parameter]
    public string Class { get; set; }
    [Parameter]
    public string Style { get; set; }
    [Parameter]
    public string Height { get; set; }

    [Inject]
    private IDialogService DialogService { get; set; }
    [Inject]
    private ILocalizerService LocalizerService { get; set; }

    private ItemCollection<AttributeItem> Attributes { get; } = new();
    // ReSharper disable once UnusedAutoPropertyAccessor.Local
    private MudDataGrid<AttributeItem> Grid { get; set; }

    private Localizer Localizer => LocalizerService.Localizer;

    private void SetupAttributes()
    {
        Attributes.Clear();
        if (Item == null || Item.Attributes == null || !Item.Attributes.Any())
        {
            return;
        }

        foreach (var attribute in Item.Attributes)
        {
            Attributes.Add(new(
                name: attribute.Key,
                valueType: attribute.Value.GetAttributeType(),
                value: attribute.Value?.ToString()));
        }
    }

    private string LocalizedItemName => Localizer.Key(Item.GetType().Name, Item.GetType().Name);
    private string LocalizedItemFullName =>
        $"{LocalizedItemName} {Localizer.Attribute.Attribute}";

    private async Task AddAttributeAsync()
    {
        // dialog parameters
        var parameters = new DialogParameters
        {
            { nameof(AttributeDialog.Attributes), Item.Attributes }
        };

        // attribute create dialog
        var dialog = await (await DialogService.ShowAsync<AttributeDialog>(
            Localizer.Item.AddTitle(LocalizedItemFullName), parameters)).Result;
        if (dialog == null || dialog.Canceled)
        {
            return;
        }

        // new attribute
        var item = dialog.Data as AttributeItem;
        if (item == null)
        {
            return;
        }

        // add object attribute
        Item.Attributes ??= new();
        Item.Attributes[item.Name] = item.Value;
        Attributes.Add(item);
    }

    private async Task EditAttributeAsync(AttributeItem item)
    {
        if (item == null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        // existing
        if (!Item.Attributes.ContainsKey(item.Name) ||
            !Attributes.Contains(item))
        {
            return;
        }

        // dialog parameters
        var parameters = new DialogParameters
        {
            { nameof(AttributeDialog.Attribute), item }
        };

        // attribute edit dialog
        var dialog = await (await DialogService.ShowAsync<AttributeDialog>(
            Localizer.Item.EditTitle(LocalizedItemFullName), parameters)).Result;
        if (dialog == null || dialog.Canceled)
        {
            return;
        }

        // update object attribute
        Item.Attributes[item.Name] = item.Value;
    }

    private async Task RemoveAttributeAsync(AttributeItem item)
    {
        if (item == null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        // existing
        if (!Item.Attributes.ContainsKey(item.Name) ||
            !Attributes.Contains(item))
        {
            return;
        }

        // confirmation
        if (!await DialogService.ShowDeleteMessageBoxAsync(
                Localizer,
                Localizer.Item.RemoveTitle(LocalizedItemFullName),
                Localizer.Item.RemoveQuery(item.Name)))
        {
            return;
        }

        // remove object attribute
        Item.Attributes.Remove(item.Name);
        Attributes.Remove(item);
    }

    protected override async Task OnInitializedAsync()
    {
        // attributes
        SetupAttributes();
        await base.OnInitializedAsync();
    }

    public void Dispose()
    {
        Attributes?.Dispose();
    }
}
