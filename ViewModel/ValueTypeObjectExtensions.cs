
namespace PayrollEngine.WebApp.ViewModel;

public static class ValueTypeObjectExtensions
{
    public static object GetValueByValueType(this IVariantValue obj)
    {
        if (obj.ValueType.IsString())
        {
            return obj.ValueAsString;
        }
        if (obj.ValueType.IsDateTime())
        {
            return obj.ValueAsDateTime;
        }
        if (obj.ValueType.IsInteger())
        {
            return obj.ValueAsInteger;
        }
        if (obj.ValueType.IsDecimal())
        {
            return obj.ValueAsDecimal;
        }
        if (obj.ValueType.IsBoolean())
        {
            return obj.ValueAsBoolean;
        }
        return obj.Value;
    }
}