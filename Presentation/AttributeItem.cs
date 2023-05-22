using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;

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
                return JsonSerializer.Deserialize<decimal>(stringValue);
            }
            if (Value is decimal decimalValue)
            {
                return decimalValue;
            }
            return default;
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
                return JsonSerializer.Deserialize<bool>(stringValue);
            }
            if (Value is bool boolValue)
            {
                return boolValue;
            }
            return default;
        }
        set
        {
            if (ValueType != AttributeValueType.Boolean)
            {
                throw new InvalidOperationException();
            }
            Value = JsonSerializer.Serialize(value);
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