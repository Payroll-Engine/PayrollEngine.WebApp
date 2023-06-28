using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;
using PayrollEngine.WebApp.ViewModel;
using Task = System.Threading.Tasks.Task;

namespace PayrollEngine.WebApp.Presentation.Regulation.Component;

public partial class MultiEnumListBox<T> : IRegulationInput
{
    [Parameter]
    public RegulationEditContext EditContext { get; set; }
    [Parameter]
    public IRegulationItem Item { get; set; }
    [Parameter]
    public RegulationField Field { get; set; }
    [Parameter]
    public EventCallback<object> ValueChanged { get; set; }

    protected List<string> EnumValues { get; set; }
    protected List<T> Value { get; set; }

    protected List<T> FieldValue
    {
        get => Item.GetPropertyValue<List<T>>(Field.PropertyName);
        set => Item.SetPropertyValue(Field.PropertyName, value);
    }

    protected bool IsBaseValue { get; set; }

    private IEnumerable<string> SelectedValues { get; set; } = new HashSet<string>();
    //private T SelectedValue { get; set; }

    #region Value

    public string ValuesAsString
    {
        get
        {
            var values = FieldValue;
            if (values == null || !values.Any())
            {
                return null;
            }
            return string.Join(',', values.Select(x => x.ToString().ToPascalSentence()));
        }
    }

    private string GetMultiSelectionText(List<string> selectedValues)
    {
        if (!selectedValues.Any())
        {
            return Localizer.Shared.None;
        }
        return string.Join(", ", selectedValues.Select(x => x.ToString().ToPascalSentence()));
    }

    private async Task SelectionChangedAsync(IEnumerable<string> value)
    {
        Value = value.Select(x => (T)Enum.Parse(typeof(T), x)).ToList();
        await SetFieldValue(Value);
    }

    private async Task SetFieldValue(List<T> value)
    {
        FieldValue = value;
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
        if (value == null && Field.HasBaseValues)
        {
            value = GetBaseValue();
        }
        Value = value;

        // selection
        SelectedValues = Value?.Select(x => x.ToString()).ToHashSet();
    }

    protected List<T> GetBaseValue() =>
        Item.GetBaseValue<List<T>>(Field.PropertyName);

    private List<Tuple<T, string>> GetEnumValues()
    {
        var enumItems = Enum.GetValues(typeof(T)).Cast<T>().ToList();
        List<Tuple<T, string>> values = new();
        foreach (var item in enumItems)
        {
            var text = Localizer != null ? Localizer.FromEnum(item) : item.ToString().ToPascalSentence();
            values.Add(new(item, text));
        }
        return values;
    }

    #endregion

    #region Lifecycle

    private void UpdateState()
    {
        var value = Value;

        // base value
        var isBaseValue = false;
        if (Field.HasBaseValues && value != null)
        {
            var baseValue = GetBaseValue();
            isBaseValue = CompareTool.EqualLists(value, baseValue);
        }
        IsBaseValue = isBaseValue;

        StateHasChanged();
    }

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
