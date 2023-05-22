using System;
using System.Linq;

namespace PayrollEngine.WebApp;

public static class TypeExtensions
{
    /// <summary>
    /// Merge all property values from one class instance to another. Only nullable properties are copied.
    /// </summary>
    /// <typeparam name="T">Type of class to be merged</typeparam>
    /// <param name="target">Target instance</param>
    /// <param name="mergeSource">Source instance</param>
    public static void MergeReferences<T>(this T target, T mergeSource)
    {
        if (mergeSource == null)
        {
            // abort if merge source does not exist
            return;
        }

        // get properties which should be copied with following criteria: nullable types OR non value types (to include string which is reference type)
        var properties = target.GetType().GetProperties()
            .Where(prop => prop.CanRead && prop.CanWrite && (prop.PropertyType.IsNullable() || !prop.PropertyType.IsValueType));

        foreach (var property in properties)
        {
            var propertyName = property.Name;
            var sourceProperty = mergeSource.GetType().GetProperty(propertyName);
            // if property is not found in source property, continue (this should never happen since both source and target MUST be of same type T
            if (sourceProperty != null)
            {
                var sourceValue = sourceProperty.GetValue(mergeSource);
                // check if source value is null
                if (sourceValue != null)
                {
                    // retrieve target property info
                    var targetProperty = target.GetType().GetProperty(propertyName);
                    if (targetProperty != null)
                    {
                        // finally set value to target if property exists (which, again, should never happen if target and source are of same type T
                        targetProperty.SetValue(target, sourceValue);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Check if two types are the same taking into account their underlying type if one of them or both are nullable.
    /// </summary>
    /// <param name="sourceType">Source type</param>
    /// <param name="targetType">Type to compare</param>
    /// <returns>Returns true uf both types (or their underlying type) are the same</returns>
    public static bool SameUnderlyingType(this Type sourceType, Type targetType)
    {
        // cant compare if either type is null
        if (sourceType == null || targetType == null)
        {
            return false;
        }

        // if types are nullable, get underlying type
        if (sourceType.IsNullable())
        {
            sourceType = Nullable.GetUnderlyingType(sourceType);
        }
        if (targetType.IsNullable())
        {
            targetType = Nullable.GetUnderlyingType(targetType);
        }

        return sourceType == targetType;
    }
}