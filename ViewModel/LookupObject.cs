using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text.Json;

namespace PayrollEngine.WebApp.ViewModel;

[SuppressMessage("ReSharper", "LocalizableElement")]
public class LookupObject
{
    private readonly Dictionary<string, JsonElement> values = new();
    public decimal? RangeValue { get; }

    private CultureInfo TenantCulture { get; }
    private IValueFormatter ValueFormatter { get; }
    private string ValuePropertyName { get; }
    private string TextPropertyName { get; }

    private readonly object lookupValue;

    public object Value
    {
        get => lookupValue;
        private init
        {
            if (value is string stringValue)
            {
                lookupValue = ValueConvert.ToValue(stringValue, ValueType.String, TenantCulture);
            }
            else
            {
                lookupValue = value;
            }
        }
    }

    public string Text { get; }

    public LookupObject()
    {
    }

    public LookupObject(JsonElement element, IValueFormatter valueFormatter, ValueType valueType,
        CultureInfo tenantCulture, decimal? rangeValue, string valuePropertyName = null, string textPropertyName = null)
    {
        TenantCulture = tenantCulture ?? throw new ArgumentNullException(nameof(tenantCulture));
        ValueFormatter = valueFormatter ?? throw new ArgumentNullException(nameof(valueFormatter));

        // properties
        var properties = new List<string>();
        foreach (var item in element.EnumerateObject())
        {
            properties.Add(item.Name);
            values[item.Name] = item.Value;
        }
        if (!properties.Any())
        {
            throw new ArgumentException("Object without properties is not supported", nameof(element));
        }

        // range
        RangeValue = rangeValue;

        // value property
        if (!string.IsNullOrWhiteSpace(valuePropertyName) && !properties.Contains(valuePropertyName))
        {
            throw new ArgumentException($"Unknown property {valuePropertyName}", nameof(valuePropertyName));
        }
        ValuePropertyName = valuePropertyName ?? properties.First();
        Value = values[ValuePropertyName].GetValue();

        // text property
        if (!string.IsNullOrWhiteSpace(textPropertyName) && !properties.Contains(textPropertyName))
        {
            throw new ArgumentException($"Unknown property {textPropertyName}", nameof(textPropertyName));
        }
        TextPropertyName = textPropertyName ?? properties.First();

        Text = string.IsNullOrWhiteSpace(textPropertyName) ?
            // no text property: value as text
            ValueFormatter.ToString(Value, valueType, tenantCulture) :
            // text property
            values[TextPropertyName].GetString();
    }

    public string GetStringValue(string propertyName)
    {
        // text column
        if (string.Equals(propertyName, TextPropertyName))
        {
            return Text;
        }
        return GetPropertyValue(propertyName)?.ToString();
    }

    public decimal? GetDecimalValue(string propertyName) =>
        GetPropertyValue<decimal>(propertyName);

    public int? GetIntegerValue(string propertyName) =>
        GetPropertyValue<int>(propertyName);

    public bool? GetBooleanValue(string propertyName) =>
        GetPropertyValue<bool>(propertyName);

    public DateTime? GetDateTimeValue(string propertyName) =>
        GetPropertyValue<DateTime>(propertyName);

    private T GetPropertyValue<T>(string propertyName)
    {
        var value = GetPropertyValue(propertyName);
        if (value == null || !(value is T typeValue))
        {
            return default;
        }
        return typeValue;
    }

    private object GetPropertyValue(string propertyName)
    {
        if (string.IsNullOrWhiteSpace(propertyName))
        {
            // assume the first/unique property of the lookup value
            if (values.Any())
            {
                propertyName = values.First().Key;
            }
        }
        if (string.IsNullOrWhiteSpace(propertyName) || !values.ContainsKey(propertyName))
        {
            throw new PayrollException($"Unknown lookup property {propertyName}");
        }
        return values[propertyName].GetValue();
    }
}