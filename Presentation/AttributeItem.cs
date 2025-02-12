using System;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace PayrollEngine.WebApp.Presentation;

public class AttributeItem
{
    [Required]
    public string Name { get; set; }

    public AttributeValueType ValueType { get; set; }

    [Required]
    public object Value { get; set; }

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
                if (decimal.TryParse(stringValue, out var value))
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

    public AttributeItem()
    {
    }

    public AttributeItem(AttributeItem copySource)
    {
        Name = copySource.Name;
        ValueType = copySource.ValueType;
        Value = copySource.Value;
    }

    public AttributeItem(string name, AttributeValueType valueType, string value)
    {
        Name = name;
        ValueType = valueType;
        Value = value;
    }
}