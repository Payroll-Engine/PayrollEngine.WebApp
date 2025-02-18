namespace PayrollEngine.WebApp.ViewModel;

/// <summary>
/// Extension methods for <see cref="IVariantValue"/>
/// </summary>
public static class VariantValueExtensions
{
    /// <summary>
    /// Get value by value type
    /// </summary>
    /// <param name="value">Variant value</param>
    public static object GetValueByValueType(this IVariantValue value)
    {
        if (value.ValueType.IsString())
        {
            return value.ValueAsString;
        }
        if (value.ValueType.IsDateTime())
        {
            return value.ValueAsDateTime;
        }
        if (value.ValueType.IsInteger())
        {
            return value.ValueAsInteger;
        }
        if (value.ValueType.IsDecimal())
        {
            return value.ValueAsDecimal;
        }
        if (value.ValueType.IsBoolean())
        {
            return value.ValueAsBoolean;
        }
        return value.Value;
    }
}