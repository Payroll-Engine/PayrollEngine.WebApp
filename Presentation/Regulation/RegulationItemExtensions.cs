using System;
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
    /// <param name="item">The regulation item</param>
    extension(IRegulationItem item)
    {
        /// <summary>
        /// Test for new object
        /// </summary>
        public bool IsNew() =>
            item != null && item.InheritanceType == RegulationInheritanceType.New;

        /// <summary>
        /// Test for derived object
        /// </summary>
        public bool IsDerived() =>
            item != null && item.InheritanceType == RegulationInheritanceType.Derived;

        /// <summary>
        /// Test for base object
        /// </summary>
        public bool IsBase() =>
            item != null && item.InheritanceType == RegulationInheritanceType.Base;

        /// <summary>
        /// Test for field localizations
        /// </summary>
        /// <param name="fieldName">The field name</param>
        public bool IsLocalizable(string fieldName) =>
            item != null && item.GetType().IsLocalizable(fieldName);

        /// <summary>
        /// Get inheritance values
        /// </summary>
        /// <param name="fieldName">The field name</param>
        private List<RegulationItemValue> GetInheritanceValues(string fieldName)
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
        /// <param name="fieldName">The field name</param>
        private List<RegulationItemValue> GetBaseValues(string fieldName) =>
            item.BaseItem != null ? item.BaseItem.GetInheritanceValues(fieldName) : [];

        /// <summary>
        /// Get base values
        /// </summary>
        /// <param name="fieldName">The field name</param>
        public T GetBaseValue<T>(string fieldName)
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
                    try
                    {
                        return (T)baseValue;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                }
                item = item.BaseItem;
            }
            return default;
        }
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

    /// <param name="item">Regulation item</param>
    extension(IRegulationItem item)
    {
        /// <summary>
        /// Get item help
        /// </summary>
        /// <param name="field">Regulation field</param>
        /// <param name="derivedHelp">Derived help</param>
        public string GetItemHelp(RegulationField field,
            string derivedHelp = null)
        {
            var help = derivedHelp ?? field.Help;
            if (!field.HasBaseValues)
            {
                return help;
            }
            var baseValues = item.GetBaseValues(field.PropertyName);
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
        /// <param name="field">Regulation field</param>
        /// <param name="localizer">Localizer</param>
        /// <param name="addRequiredMarker">Required marker</param>
        public string GetItemLabel(RegulationField field,
            Localizer localizer, bool addRequiredMarker = false)
        {
            var label = field.Label ?? field.PropertyName.ToPascalSentence();

            // base field
            if (item.IsBase())
            {
                return $"{label} ({localizer.Item.BaseField}) ";
            }

            // init field
            if (field.ReadOnly && (item.IsNew() || item.Id != 0))
            {
                return $"{label} ({localizer.Item.InitOnlyField}) ";
            }

            // read only field
            if (item.IsDerived() && field.KeyField)
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
        /// <param name="field">Regulation field</param>
        public bool IsReadOnlyField(RegulationField field)
        {
            // no edit of base fields
            if (item.IsBase())
            {
                return true;
            }

            // disable changes on key and read only fields on derived fields
            if (item.IsDerived())
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
        /// <param name="field">Regulation field</param>
        public bool IsClearableField(RegulationField field) =>
            !item.IsReadOnlyField(field);
    }
}