using System;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace PayrollEngine.WebApp.Presentation;

/// <summary>
/// Attribute item
/// </summary>
public class AttributeItem
{
    /// <summary>
    /// Attribute name
    /// </summary>
    [Required]
    public string Name { get; set; }

    /// <summary>
    /// Attribute value type
    /// </summary>
    public AttributeValueType ValueType { get; set; }

    /// <summary>
    /// Attribute value
    /// </summary>
    [Required]
    public object Value { get; set; }

    /// <summary>
    /// Attribute value as string
    /// </summary>
    [JsonIgnore]
    public string ValuesAsString
    {
        get
        {
            if (ValueType != AttributeValueType.String)
            {
                throw new InvalidOperationException();
            }
            return Value as string;
        }
        set
        {
            if (ValueType != AttributeValueType.String)
            {
                throw new InvalidOperationException();
            }
            Value = value;
        }
    }

    /// <summary>
    /// Attribute value as number
    /// </summary>
    [JsonIgnore]
    public decimal ValuesAsNumber
    {
        get
        {
            if (ValueType != AttributeValueType.Numeric)
            {
                throw new InvalidOperationException();
            }
            if (Value is string stringValue && !string.IsNullOrWhiteSpace(stringValue))
            {
                if (decimal.TryParse(stringValue, CultureInfo.InvariantCulture, out var value))
                {
                    return value;
                }
            }
            if (Value is decimal decimalValue)
            {
                return decimalValue;
            }
            return 0;
        }
        set
        {
            if (ValueType != AttributeValueType.Numeric)
            {
                throw new InvalidOperationException();
            }
            Value = value;
        }
    }

    /// <summary>
    /// Attribute value as boolean
    /// </summary>
    [JsonIgnore]
    public bool ValuesAsBoolean
    {
        get
        {
            if (ValueType != AttributeValueType.Boolean)
            {
                throw new InvalidOperationException();
            }
            if (Value is string stringValue && !string.IsNullOrWhiteSpace(stringValue))
            {
                if (bool.TryParse(stringValue, out var value))
                {
                    return value;
                }
            }
            if (Value is bool boolValue)
            {
                return boolValue;
            }
            return false;
        }
        set
        {
            if (ValueType != AttributeValueType.Boolean)
            {
                throw new InvalidOperationException();
            }
            Value = value;
        }
    }

    /// <summary>
    /// Default constructor
    /// </summary>
    public AttributeItem()
    {
    }

    /// <summary>
    /// Copy constructor
    /// </summary>
    /// <param name="copySource">Copy source</param>
    public AttributeItem(AttributeItem copySource)
    {
        Name = copySource.Name;
        ValueType = copySource.ValueType;
        Value = copySource.Value;
    }

    /// <summary>
    /// Value constructor
    /// </summary>
    /// <param name="name">Attribute name</param>
    /// <param name="valueType">Attribute value type</param>
    /// <param name="value">Attribute value</param>
    public AttributeItem(string name, AttributeValueType valueType, string value)
    {
        Name = name;
        ValueType = valueType;
        Value = value;
    }
}