﻿using System;
using System.Linq;
using System.Text;
using System.Collections;
using System.Globalization;
using System.Collections.Generic;
using PayrollEngine.WebApp.Shared;
using PayrollEngine.WebApp.ViewModel;

namespace PayrollEngine.WebApp.Presentation.Regulation;

/// <summary>
/// extension methods for <see cref="IRegulationItem"/>
/// </summary>
public static class RegulationItemExtensions
{
    /// <summary>
    /// Test for new object
    /// </summary>
    /// <param name="item">The regulation item</param>
    public static bool IsNew(this IRegulationItem item) =>
        item != null && item.InheritanceType == RegulationInheritanceType.New;

    /// <summary>
    /// Test for derived object
    /// </summary>
    /// <param name="iem">The regulation item</param>
    public static bool IsDerived(this IRegulationItem iem) =>
        iem != null && iem.InheritanceType == RegulationInheritanceType.Derived;

    /// <summary>
    /// Test for base object
    /// </summary>
    /// <param name="item">The regulation item</param>
    public static bool IsBase(this IRegulationItem item) =>
        item != null && item.InheritanceType == RegulationInheritanceType.Base;

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
    private static List<RegulationItemValue> GetInheritanceValues(this IRegulationItem item,
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
        obj.BaseItem != null ? GetInheritanceValues(obj.BaseItem, fieldName) : [];

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

    /// <summary>
    /// Test for empty value
    /// </summary>
    /// <param name="value">Value to test</param>
    private static bool IsEmptyValue(object value)
    {
        if (value == null)
        {
            return false;
        }

        return value is IList listValue && listValue.Count == 0 ||
               value is IDictionary dictValue && dictValue.Count == 0;
    }

    /// <summary>
    /// Get item help
    /// </summary>
    /// <param name="item">Regulation item</param>
    /// <param name="field">Regulation field</param>
    /// <param name="derivedHelp">Derived help</param>
    public static string GetItemHelp(this IRegulationItem item, RegulationField field,
        string derivedHelp = null)
    {
        var help = derivedHelp ?? field.Help;
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

    /// <summary>
    /// Get item label
    /// </summary>
    /// <param name="item">Regulation item</param>
    /// <param name="field">Regulation field</param>
    /// <param name="localizer">Localizer</param>
    /// <param name="addRequiredMarker">Required marker</param>
    public static string GetItemLabel(this IRegulationItem item, RegulationField field,
        Localizer localizer, bool addRequiredMarker = false)
    {
        var label = field.Label ?? field.PropertyName.ToPascalSentence();

        // base field
        if (IsBase(item))
        {
            return $"{label} ({localizer.Item.BaseField}) ";
        }

        // init field
        if (field.ReadOnly && (IsNew(item) || item.Id != 0))
        {
            return $"{label} ({localizer.Item.InitOnlyField}) ";
        }

        // read only field
        if (IsDerived(item) && field.KeyField)
        {
            return $"{label} ({localizer.Item.ReadOnlyField}) ";
        }

        // label spacing
        label += (" ");

        // required
        if (addRequiredMarker && field.Required)
        {
            label = label.EnsureEnd("*");
        }

        return label;
    }

    /// <summary>
    /// Test for read only field
    /// </summary>
    /// <param name="item">Regulation item</param>
    /// <param name="field">Regulation field</param>
    public static bool IsReadOnlyField(this IRegulationItem item, RegulationField field)
    {
        // no edit of base fields
        if (IsBase(item))
        {
            return true;
        }

        // disable changes on key and read only fields on derived fields
        if (IsDerived(item))
        {
            // derived key field
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

    /// <summary>
    /// Test for clearable field
    /// </summary>
    /// <param name="item">Regulation item</param>
    /// <param name="field">Regulation field</param>
    public static bool IsClearableField(this IRegulationItem item, RegulationField field) =>
        !IsReadOnlyField(item, field);
}