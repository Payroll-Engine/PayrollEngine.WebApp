using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using PayrollEngine.WebApp.ViewModel;

namespace PayrollEngine.WebApp.Presentation.Regulation;

public static class RegulationItemExtensions
{
    /// <summary>
    /// Test for new object
    /// </summary>
    /// <param name="item">The regulation item</param>
    public static bool IsNew(this IRegulationItem item) =>
        item != null && item.InheritanceType == RegulationInheritanceType.New;

    /// <summary>
    /// Test for override object
    /// </summary>
    /// <param name="iem">The regulation item</param>
    public static bool IsOverride(this IRegulationItem iem) =>
        iem != null && iem.InheritanceType == RegulationInheritanceType.Override;

    /// <summary>
    /// Test for derived object
    /// </summary>
    /// <param name="item">The regulation item</param>
    public static bool IsDerived(this IRegulationItem item) =>
        item != null && item.InheritanceType == RegulationInheritanceType.Derived;

    /// <summary>
    /// Test for field localizations
    /// </summary>
    /// <param name="item">The regulation item</param>
    /// <param name="fieldName">The field name</param>
    public static bool IsLocalizable(this IRegulationItem item, string fieldName) =>
        item != null && item.GetType().IsLocalizable(fieldName);

    /// <summary>
    /// Get inheritance values
    /// </summary>
    /// <param name="item">The regulation item</param>
    /// <param name="fieldName">The field name</param>
    public static List<RegulationItemValue> GetInheritanceValues(this IRegulationItem item,
        string fieldName)
    {
        if (string.IsNullOrWhiteSpace(fieldName))
        {
            throw new ArgumentException(nameof(fieldName));
        }

        var values = new List<RegulationItemValue>();
        while (item != null)
        {
            var value = item.GetPropertyValue(fieldName);
            if (value != null)
            {
                if (!IsEmptyValue(value))
                {
                    values.Add(new(item, value));
                }
            }
            item = item.BaseItem;
        }
        return values;
    }

    /// <summary>
    /// Get base values
    /// </summary>
    /// <param name="obj">The regulation item</param>
    /// <param name="fieldName">The field name</param>
    private static List<RegulationItemValue> GetBaseValues(this IRegulationItem obj, string fieldName) =>
        obj.BaseItem != null ? GetInheritanceValues(obj.BaseItem, fieldName) : new();

    /// <summary>
    /// Get base values
    /// </summary>
    /// <param name="item">The regulation item</param>
    /// <param name="fieldName">The field name</param>
    public static T GetBaseValue<T>(this IRegulationItem item, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(fieldName))
        {
            throw new ArgumentException(nameof(fieldName));
        }
        if (item.BaseItem == null)
        {
            return default;
        }

        item = item.BaseItem;
        while (item != null)
        {
            var baseValue = item.GetPropertyValue(fieldName);
            if (!IsEmptyValue(baseValue))
            {
                return (T)baseValue;
            }
            item = item.BaseItem;
        }
        return default;
    }

    private static bool IsEmptyValue(object value)
    {
        if (value == null)
        {
            return false;
        }

        return value is IList listValue && listValue.Count == 0 ||
               value is IDictionary dictValue && dictValue.Count == 0;
    }

    public static string GetItemHelp(this IRegulationItem item, RegulationField field,
        string overrideHelp = null)
    {
        var help = overrideHelp ?? field.Help;
        if (!field.HasBaseValues)
        {
            return help;
        }
        var baseValues = GetBaseValues(item, field.PropertyName);
        if (!baseValues.Any())
        {
            return help;
        }

        // list base values
        var separator = $"  {(char)short.Parse("2022", NumberStyles.AllowHexSpecifier)}  ";
        var buffer = new StringBuilder();
        if (!string.IsNullOrWhiteSpace(help))
        {
            buffer.Append(help);
            buffer.Append(separator);
        }
        foreach (var baseValue in baseValues)
        {
            if (baseValue != baseValues.First())
            {
                buffer.Append(separator);
            }
            buffer.Append(baseValue.Text);
        }
        return buffer.ToString();
    }

    public static string GetItemLabel(this IRegulationItem item, RegulationField field)
    {
        var label = field.Label ?? field.PropertyName.ToPascalSentence();

        // derived field
        if (IsDerived(item))
        {
            return $"{label} (derived) ";
        }

        // init field
        if (field.ReadOnly && (IsNew(item) || item.Id != 0))
        {
            return $"{label} (init only) ";
        }

        // read only field
        if (IsOverride(item) && field.KeyField)
        {
            return $"{label} (read only) ";
        }

        return label + " ";
    }

    public static string GetItemActionLabel(this IRegulationItem item, RegulationField field)
    {
        // base name
        var name = field.PropertyName;
        if (field.IsAction)
        {
            name = name.RemoveFromEnd("Expression").EnsureEnd("Action");
        }

        var label = name.ToPascalSentence();
        return label + " ";
    }

    public static bool IsReadOnlyField(this IRegulationItem item, RegulationField field)
    {
        // no edit of derived fields
        if (IsDerived(item))
        {
            return true;
        }

        // disable changes on key and read only fields on override fields
        if (IsOverride(item))
        {
            // override key field
            if (field.KeyField)
            {
                return true;
            }
        }

        // read-only field only on new created object
        if (field.ReadOnly)
        {
            return item.Id != 0;
        }

        // derived fields and new object fields are not read only
        return false;
    }

    public static bool IsClearableField(this IRegulationItem item, RegulationField field) =>
        !IsReadOnlyField(item, field);
}