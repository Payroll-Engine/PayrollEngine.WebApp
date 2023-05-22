using System.Text.Json;

namespace PayrollEngine.WebApp.Presentation;

public static class AttributeValueTypeExtensions
{
    public static AttributeValueType GetAttributeType(this ValueType valueType)
    {
        if (valueType.IsNumber())
        {
            return AttributeValueType.Numeric;
        }
        if (valueType.IsBoolean())
        {
            return AttributeValueType.Boolean;
        }
        return AttributeValueType.String;
    }

    public static AttributeValueType GetAttributeType(this object value, AttributeValueType defaultValue = default)
    {
        if (value == null)
        {
            return defaultValue;
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

}