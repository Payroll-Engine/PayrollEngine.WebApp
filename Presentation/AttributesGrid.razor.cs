using System;
using System.Linq;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Task = System.Threading.Tasks.Task;

namespace PayrollEngine.WebApp.Presentation;

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

    protected ItemCollection<AttributeItem> Attributes { get; set; } = new();
    protected MudDataGrid<AttributeItem> Grid { get; set; }

    private void SetupAttributes()
    {
        Attributes.Clear();
        if (!Item.Attributes.Any())
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

    private string ItemName => Item.GetType().Name.ToPascalSentence();

    protected async Task AddAttributeAsync()
    {
        // dialog parameters
        var parameters = new DialogParameters
        {
            { nameof(AttributeDialog.Attributes), Item.Attributes }
        };

        // attribute create dialog
        var dialog = await (await DialogService.ShowAsync<AttributeDialog>($"Add {ItemName} attribute", parameters)).Result;
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
        Item.Attributes[item.Name] = item.Value;
        Attributes.Add(item);
    }

    protected async Task EditAttributeAsync(AttributeItem item)
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
        var dialog = await (await DialogService.ShowAsync<AttributeDialog>($"Edit {ItemName} attribute", parameters)).Result;
        if (dialog == null || dialog.Canceled)
        {
            return;
        }

        // update object attribute
        Item.Attributes[item.Name] = item.Value;
    }

    protected async Task DeleteAttributeAsync(AttributeItem item)
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
                "Delete attribute",
                $"Delete {item.Name} permanently?"))
        {
            return;
        }

        // delete object attribute
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
