using System;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using PayrollEngine.WebApp.Presentation.Component;
using PayrollEngine.WebApp.Shared;
using PayrollEngine.WebApp.ViewModel;
using Task = System.Threading.Tasks.Task;

namespace PayrollEngine.WebApp.Presentation.Regulation.Component;

public partial class RegulationAttributeGrid : IRegulationInput, IDisposable
{
    [Parameter]
    public RegulationEditContext EditContext { get; set; }
    [Parameter]
    public IRegulationItem Item { get; set; }
    [Parameter]
    public RegulationField Field { get; set; }
    [Parameter]
    public EventCallback<object> ValueChanged { get; set; }

    [Inject]
    private IDialogService DialogService { get; set; }
    [Inject]
    private ILocalizerService LocalizerService { get; set; }

    private Localizer Localizer => LocalizerService.Localizer;
    private ItemCollection<AttributeItem> Attributes { get; set; } = new();
    private MudDataGrid<AttributeItem> Grid { get; set; }

    private string LocalizedItemName => Localizer.GroupKey(Item.ItemType.ToString());
    private string LocalizedItemFullName =>
        $"{LocalizedItemName} {Localizer.Attribute.Attribute}";
    private static string AttributesFieldName => nameof(IAttributeObject.Attributes);

    #region Value

    private Dictionary<string, object> FieldValue
    {
        get => Item.GetPropertyValue<Dictionary<string, object>>(AttributesFieldName);
        set => Item.SetPropertyValue(AttributesFieldName, value);
    }

    private async Task SetFieldValue()
    {
        // field value
        var fieldValue = new Dictionary<string, object>();
        foreach (var item in Attributes)
        {
            fieldValue.Add(item.Name, item.Value);
        }
        if (CompareTool.EqualDictionaries<string, object>(FieldValue, fieldValue))
        {
            return;
        }
        FieldValue = fieldValue;
        ApplyFieldValue();

        // notifications
        await ValueChanged.InvokeAsync(FieldValue);
    }

    private void ApplyFieldValue()
    {
        // value
        var value = FieldValue;

        var attributes = new List<AttributeItem>();
        if (value != null)
        {
            foreach (var o in value)
            {
                attributes.Add(new()
                {
                    Name = o.Key,
                    Value = o.Value?.ToString(),
                    ValueType = GetValueType(o.Value)
                });
            }
        }
        Attributes = new(attributes);
        StateHasChanged();
    }

    #endregion

    #region Actions

    private async Task AddAttributeAsync()
    {
        // dialog parameters
        var parameters = new DialogParameters
        {
            { nameof(AttributeDialog.Attributes), Item.Attributes }
        };

        // attribute create dialog
        var dialog = await (await DialogService.ShowAsync<AttributeDialog>(
            Localizer.Item.AddHelp(LocalizedItemFullName), parameters)).Result;
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

        // add attribute
        Attributes.Add(item);
        await SetFieldValue();
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

        // edit copy
        var editItem = new AttributeItem(item);

        // dialog parameters
        var parameters = new DialogParameters
        {
            { nameof(AttributeDialog.Attribute), editItem }
        };

        // attribute edit dialog
        var dialog = await (await DialogService.ShowAsync<AttributeDialog>(
            Localizer.Item.EditHelp(LocalizedItemFullName), parameters)).Result;
        if (dialog == null || dialog.Canceled)
        {
            return;
        }

        // replace attribute
        Attributes.Remove(item);
        Attributes.Add(editItem);
        await SetFieldValue();
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
                Localizer.Item.RemoveQuery(LocalizedItemFullName)))
        {
            return;
        }

        // remove attribute
        Attributes.Remove(item);
        await SetFieldValue();
    }

    private static AttributeValueType GetValueType(object value)
    {
        if (value == null)
        {
            return default;
        }

        if (value is JsonElement jsonElement)
        {
            value = jsonElement.GetValue();
        }

        // numeric
        if (value is int || value is decimal || value is float)
        {
            return AttributeValueType.Numeric;
        }

        // boolean
        if (value is bool)
        {
            return AttributeValueType.Boolean;
        }

        // string
        return AttributeValueType.String;
    }

    #endregion

    #region Lifecycle

    private IRegulationItem lastObject;

    protected override async Task OnInitializedAsync()
    {
        lastObject = Item;
        ApplyFieldValue();
        await base.OnInitializedAsync();
    }

    protected override async Task OnParametersSetAsync()
    {
        if (lastObject != Item)
        {
            lastObject = Item;
            ApplyFieldValue();
        }
        await base.OnParametersSetAsync();
    }

    public void Dispose()
    {
        Attributes?.Dispose();
    }

    #endregion

}
