using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;
using PayrollEngine.WebApp.ViewModel;
using Task = System.Threading.Tasks.Task;

namespace PayrollEngine.WebApp.Presentation.Regulation.Component;

public partial class CsvTextBox : IRegulationInput
{
    [Parameter]
    public RegulationEditContext EditContext { get; set; }
    [Parameter]
    public IRegulationItem Item { get; set; }
    [Parameter]
    public RegulationField Field { get; set; }
    [Parameter]
    public EventCallback<object> ValueChanged { get; set; }

    #region Value

    private string Value { get; set; }

    private List<string> FieldValue
    {
        get => Item.GetPropertyValue<List<string>>(Field.PropertyName);
        set => Item.SetPropertyValue(Field.PropertyName, value);
    }

    private static List<string> StringToList(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var tokens = value.Split(',', StringSplitOptions.RemoveEmptyEntries);
        var uniqueTokens = new HashSet<string>();
        foreach (var token in tokens)
        {
            uniqueTokens.Add(token.Trim());
        }
        return uniqueTokens.ToList();
    }

    private static string ListToString(List<string> value)
    {
        if (value == null || !value.Any())
        {
            return null;
        }
        return string.Join(',', value);
    }

    private async Task ValueChangedAsync(string value) =>
        await SetFieldValue(StringToList(value));

    private async Task SetFieldValue(List<string> value)
    {
        // field value
        var fieldValue = value;
        if (fieldValue != null && fieldValue.Any())
        {
            var baseValue = GetBaseValue();
            if (baseValue != null && baseValue.Any() &&
                !Field.Required && CompareTool.EqualDistinctLists(fieldValue, baseValue))
            {
                // reset value on non-mandatory fields
                fieldValue = null;
            }
        }
        FieldValue = fieldValue;
        ApplyFieldValue();

        // notifications
        UpdateState();
        await ValueChanged.InvokeAsync(value);
    }

    private void ApplyFieldValue()
    {
        // value
        var value = FieldValue;
        // base value
        if (value == null || !value.Any() && Field.HasBaseValues)
        {
            value = GetBaseValue();
        }
        Value = ListToString(value);
    }

    private List<string> GetBaseValue() =>
        Item.GetBaseValue<List<string>>(Field.PropertyName);

    #endregion

    #region Lifecycle

    private void UpdateState() =>
        StateHasChanged();

    private IRegulationItem lastItem;

    protected override async Task OnInitializedAsync()
    {
        lastItem = Item;
        ApplyFieldValue();
        UpdateState();
        await base.OnInitializedAsync();
    }

    protected override async Task OnParametersSetAsync()
    {
        if (lastItem != Item)
        {
            lastItem = Item;
            ApplyFieldValue();
            UpdateState();
        }
        await base.OnParametersSetAsync();
    }

    #endregion

}
