using System.Text.Json;

namespace PayrollEngine.WebApp.Presentation;

/// <summary>
/// Extension methods for <see cref="AttributeValueType" />
/// </summary>
public static class AttributeValueTypeExtensions
{
    /// <summary>
    /// Get attribute type
    /// </summary>
    /// <param name="valueType">Value type</param>
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

    /// <summary>
    /// Get attribute type
    /// </summary>
    /// <param name="value">Value</param>
    /// <param name="defaultValue">Default attribute type</param>
    /// <returns></returns>
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