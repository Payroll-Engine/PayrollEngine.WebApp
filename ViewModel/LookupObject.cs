using System;
using System.Linq;
using System.Text.Json;
using System.Globalization;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace PayrollEngine.WebApp.ViewModel;

/// <summary>
/// View model lookup object
/// </summary>
[SuppressMessage("ReSharper", "LocalizableElement")]
public class LookupObject
{
    private readonly Dictionary<string, JsonElement> values = new();
    private CultureInfo TenantCulture { get; }
    private IValueFormatter ValueFormatter { get; }
    private string ValuePropertyName { get; }
    private string TextPropertyName { get; }

    /// <summary>
    /// Range value
    /// </summary>
    public decimal? RangeValue { get; }

    private readonly object lookupValue;
    /// <summary>
    /// Lookup value
    /// </summary>
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

    /// <summary>
    /// Lookup text
    /// </summary>
    public string Text { get; }

    /// <summary>
    /// Default constructor
    /// </summary>
    public LookupObject()
    {
    }

    /// <summary>
    /// Json value constructor
    /// </summary>
    /// <param name="element">Json element</param>
    /// <param name="valueFormatter">Value formatter</param>
    /// <param name="valueType">Value type</param>
    /// <param name="tenantCulture">Tenant culture</param>
    /// <param name="rangeValue">Range value</param>
    /// <param name="valuePropertyName">Value property name</param>
    /// <param name="textPropertyName">Text property name</param>
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
            throw new ArgumentException("Object without properties is not supported.", nameof(element));
        }

        // range
        RangeValue = rangeValue;

        // value property
        if (!string.IsNullOrWhiteSpace(valuePropertyName) && !properties.Contains(valuePropertyName))
        {
            throw new ArgumentException($"Unknown property {valuePropertyName}.", nameof(valuePropertyName));
        }
        ValuePropertyName = valuePropertyName ?? properties.First();
        Value = values[ValuePropertyName].GetValue();

        // text property
        if (!string.IsNullOrWhiteSpace(textPropertyName) && !properties.Contains(textPropertyName))
        {
            throw new ArgumentException($"Unknown property {textPropertyName}.", nameof(textPropertyName));
        }
        TextPropertyName = textPropertyName ?? properties.First();

        Text = string.IsNullOrWhiteSpace(textPropertyName) ?
            // no text property: value as text
            ValueFormatter.ToString(Value, valueType, tenantCulture) :
            // text property
            values[TextPropertyName].GetString();
    }

    /// <summary>
    /// Get string value
    /// </summary>
    /// <param name="propertyName">Property name</param>
    public string GetStringValue(string propertyName)
    {
        // text column
        if (string.Equals(propertyName, TextPropertyName))
        {
            return Text;
        }
        return GetPropertyValue(propertyName)?.ToString();
    }

    /// <summary>
    /// Get decimal value
    /// </summary>
    public decimal? GetDecimalValue(string propertyName) =>
        GetPropertyValue<decimal>(propertyName);

    /// <summary>
    /// Get integer value
    /// </summary>
    public int? GetIntegerValue(string propertyName) =>
        GetPropertyValue<int>(propertyName);

    /// <summary>
    /// Get boolean value
    /// </summary>
    public bool? GetBooleanValue(string propertyName) =>
        GetPropertyValue<bool>(propertyName);

    /// <summary>
    /// Get string value
    /// </summary>
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
        if (string.IsNullOrWhiteSpace(propertyName) || !values.TryGetValue(propertyName, out var value))
        {
            throw new PayrollException($"Unknown lookup property {propertyName}.");
        }
        return value.GetValue();
    }
}