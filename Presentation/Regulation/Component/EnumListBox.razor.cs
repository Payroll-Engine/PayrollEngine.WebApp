using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;
using PayrollEngine.WebApp.ViewModel;
using Task = System.Threading.Tasks.Task;

namespace PayrollEngine.WebApp.Presentation.Regulation.Component;

public partial class EnumListBox<T> : IRegulationInput
{
    [Parameter]
    public RegulationEditContext EditContext { get; set; }
    [Parameter]
    public IRegulationItem Item { get; set; }
    [Parameter]
    public RegulationField Field { get; set; }
    [Parameter]
    public EventCallback<object> ValueChanged { get; set; }

    private T Value { get; set; }

    private object FieldValue
    {
        get => Item.GetPropertyValue(Field.PropertyName);
        set => Item.SetPropertyValue(Field.PropertyName, value);
    }

    #region Value

    private async Task ValueChangedAsync(T item) =>
        await SetFieldValue(item);

    private async Task SetFieldValue(object value)
    {
        // enum conversion
        var enumValue = ParseEnumValue(value);

        // field value
        var fieldValue = enumValue;
        if (fieldValue != null)
        {
            var baseValue = GetBaseValue();
            if (baseValue != null &&
                !Field.Required && Equals(fieldValue, baseValue))
            {
                // reset value on non-mandatory fields
                fieldValue = null;
            }
        }
        FieldValue = fieldValue;
        ApplyFieldValue();

        // notifications
        UpdateState();
        await ValueChanged.InvokeAsync(enumValue);
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
        if (value != null && !value.GetType().IsEnum)
        {
            throw new PayrollException($"Invalid value {value} for enum {typeof(T).Name}");
        }
        Value = value != null ? (T)value : default(T);
    }

    private Type GetEnumType()
    {
        var type = Item.GetType().GetProperty(Field.PropertyName)?.PropertyType;
        if (type == null)
        {
            return null;
        }

        var nullableType = Nullable.GetUnderlyingType(type);
        if (nullableType != null)
        {
            type = nullableType;
        }
        if (!type.IsEnum)
        {
            throw new PayrollException($"Invalid enum field {Field.PropertyName}");
        }
        return type;
    }

    private object ParseEnumValue(object value)
    {
        if (value == null)
        {
            return null;
        }

        // enum
        if (value.GetType().IsEnum)
        {
            return value;
        }

        // enum string
        var enumType = GetEnumType();
        if (enumType != null)
        {
            if (value is string stringValue)
            {
                return Enum.Parse(enumType, stringValue);
            }
        }

        return null;
    }

    private object GetBaseValue() =>
        Item.GetBaseValue<object>(Field.PropertyName);

    private List<Tuple<T, string>> GetEnumValues()
    {
        var enumItems = Enum.GetValues(typeof(T)).Cast<T>().ToList();
        List<Tuple<T, string>> values = [];
        foreach (var item in enumItems)
        {
            var text = Localizer != null ? Localizer.Enum(item) : item.ToString().ToPascalSentence();
            values.Add(new(item, text));
        }
        return values;
    }

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
