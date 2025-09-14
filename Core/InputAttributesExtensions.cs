using System;
using System.Text.Json;
using System.Globalization;
using System.Collections.Generic;

namespace PayrollEngine.WebApp;

/// <summary>
/// Input attribute extensions
/// </summary>
public static class InputAttributesExtensions
{

    #region Transient

    public static Dictionary<string, object> GetEditInfo(this Dictionary<string, object> attributes)
    {
        if (attributes == null || !attributes.TryGetValue(InputAttributes.EditInfo, out var attribute))
        {
            return null;
        }

        var info = new Dictionary<string, object>();
        if (attribute is JsonElement jsonElement)
        {
            var buildInfo = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonElement.ToString());
            foreach (var item in buildInfo)
            {
                info.Add(item.Key, item.Value);
            }
        }
        return info;
    }

    public static bool? GetValidity(this Dictionary<string, object> attributes, CultureInfo culture) =>
        attributes.GetBooleanAttributeValue(InputAttributes.Validity, culture.Name);

    #endregion

    #region Case General

    public static string GetIcon(this Dictionary<string, object> attributes, CultureInfo culture) =>
        attributes.GetStringAttributeValue(InputAttributes.Icon, culture.Name);

    public static CasePriority? GetPriority(this Dictionary<string, object> attributes, CultureInfo culture) =>
        attributes.GetEnumAttributeValue<CasePriority>(InputAttributes.Priority, culture.Name);

    #endregion

    #region Case Field General

    public static string GetGroup(this Dictionary<string, object> attributes, CultureInfo culture) =>
        attributes.GetStringAttributeValue(InputAttributes.Group, culture.Name);

    public static bool? GetSeparator(this Dictionary<string, object> attributes, CultureInfo culture) =>
        attributes.GetBooleanAttributeValue(InputAttributes.Separator, culture.Name);

    public static bool? GetHidden(this Dictionary<string, object> attributes, CultureInfo culture) =>
        attributes.GetBooleanAttributeValue(InputAttributes.Hidden, culture.Name);

    public static bool? GetHiddenName(this Dictionary<string, object> attributes, CultureInfo culture) =>
        attributes.GetBooleanAttributeValue(InputAttributes.HiddenName, culture.Name);

    public static FieldLayoutMode? GetFieldLayout(this Dictionary<string, object> attributes, CultureInfo culture) =>
        attributes.GetEnumAttributeValue<FieldLayoutMode>(InputAttributes.FieldLayout, culture.Name);

    public static bool? GetShowDescription(this Dictionary<string, object> attributes, CultureInfo culture) =>
        attributes.GetBooleanAttributeValue(InputAttributes.ShowDescription, culture.Name);

    public static InputVariant? GetVariant(this Dictionary<string, object> attributes, CultureInfo culture) =>
        attributes.GetEnumAttributeValue<InputVariant>(InputAttributes.Variant, culture.Name);

    #endregion

    #region Case Field Start

    public static bool? GetStartReadOnly(this Dictionary<string, object> attributes, CultureInfo culture) =>
        attributes.GetBooleanAttributeValue(InputAttributes.StartReadOnly, culture.Name);

    public static bool? GetStartHidden(this Dictionary<string, object> attributes, CultureInfo culture) =>
        attributes.GetBooleanAttributeValue(InputAttributes.StartHidden, culture.Name);

    public static DatePickerType? GetStartPickerOpen(this Dictionary<string, object> attributes, CultureInfo culture) =>
        attributes.GetEnumAttributeValue<DatePickerType>(InputAttributes.StartPickerOpen, culture.Name);

    public static DateTimePickerType? GetStartPickerType(this Dictionary<string, object> attributes, CultureInfo culture) =>
        attributes.GetEnumAttributeValue<DateTimePickerType>(InputAttributes.StartPickerType, culture.Name);

    public static string GetStartFormat(this Dictionary<string, object> attributes, CultureInfo culture)
    {
        if (!attributes.TryGetAttributeValue<object>(InputAttributes.StartFormat, out var value, culture.Name))
        {
            return null;
        }
        if (value is JsonElement jsonElement)
        {
            value = jsonElement.GetValue();
        }
        return value as string;
    }

    public static string GetStartLabel(this Dictionary<string, object> attributes, CultureInfo culture) =>
        attributes.GetStringAttributeValue(InputAttributes.StartLabel, culture.Name);

    public static string GetStartHelp(this Dictionary<string, object> attributes, CultureInfo culture) =>
        attributes.GetStringAttributeValue(InputAttributes.StartHelp, culture.Name);

    public static string GetStartRequired(this Dictionary<string, object> attributes, CultureInfo culture) =>
        attributes.GetStringAttributeValue(InputAttributes.StartRequired, culture.Name);

    #endregion

    #region Case Field End

    public static bool? GetEndReadOnly(this Dictionary<string, object> attributes, CultureInfo culture) =>
        attributes.GetBooleanAttributeValue(InputAttributes.EndReadOnly, culture.Name);

    public static bool? GetEndHidden(this Dictionary<string, object> attributes, CultureInfo culture) =>
        attributes.GetBooleanAttributeValue(InputAttributes.EndHidden, culture.Name);

    public static DatePickerType? GetEndPickerOpen(this Dictionary<string, object> attributes, CultureInfo culture) =>
        attributes.GetEnumAttributeValue<DatePickerType>(InputAttributes.EndPickerOpen, culture.Name);

    public static DateTimePickerType? GetEndPickerType(this Dictionary<string, object> attributes, CultureInfo culture) =>
        attributes.GetEnumAttributeValue<DateTimePickerType>(InputAttributes.EndPickerType, culture.Name);

    public static string GetEndFormat(this Dictionary<string, object> attributes, CultureInfo culture)
    {
        if (!attributes.TryGetAttributeValue<object>(InputAttributes.EndFormat, out var value, culture.Name))
        {
            return null;
        }
        if (value is JsonElement jsonElement)
        {
            value = jsonElement.GetValue();
        }
        return value as string;
    }

    public static string GetEndLabel(this Dictionary<string, object> attributes, CultureInfo culture) =>
        attributes.GetStringAttributeValue(InputAttributes.EndLabel, culture.Name);

    public static string GetEndHelp(this Dictionary<string, object> attributes, CultureInfo culture) =>
        attributes.GetStringAttributeValue(InputAttributes.EndHelp, culture.Name);

    public static string GetEndRequired(this Dictionary<string, object> attributes, CultureInfo culture) =>
        attributes.GetStringAttributeValue(InputAttributes.EndRequired, culture.Name);

    #endregion

    #region Case Field Value

    public static string GetValueLabel(this Dictionary<string, object> attributes, CultureInfo culture) =>
        attributes.GetStringAttributeValue(InputAttributes.ValueLabel, culture.Name);

    public static string GetValueAdornment(this Dictionary<string, object> attributes, CultureInfo culture) =>
        attributes.GetStringAttributeValue(InputAttributes.ValueAdornment, culture.Name);

    public static string GetValueHelp(this Dictionary<string, object> attributes, CultureInfo culture) =>
        attributes.GetStringAttributeValue(InputAttributes.ValueHelp, culture.Name);

    public static string GetValueMask(this Dictionary<string, object> attributes, CultureInfo culture) =>
        attributes.GetStringAttributeValue(InputAttributes.ValueMask, culture.Name);

    public static string GetValueRequired(this Dictionary<string, object> attributes, CultureInfo culture) =>
        attributes.GetStringAttributeValue(InputAttributes.ValueRequired, culture.Name);

    public static bool? GetValueReadOnly(this Dictionary<string, object> attributes, CultureInfo culture) =>
        attributes.GetBooleanAttributeValue(InputAttributes.ValueReadOnly, culture.Name);

    public static DatePickerType? GetValuePickerOpen(this Dictionary<string, object> attributes, CultureInfo culture) =>
        attributes.GetEnumAttributeValue<DatePickerType>(InputAttributes.ValuePickerOpen, culture.Name);

    public static bool? GetValuePickerStatic(this Dictionary<string, object> attributes, CultureInfo culture) =>
        attributes.GetBooleanAttributeValue(InputAttributes.ValuePickerStatic, culture.Name);

    public static TimePickerType? GetValueTimePicker(this Dictionary<string, object> attributes, CultureInfo culture) =>
        attributes.GetEnumAttributeValue<TimePickerType>(InputAttributes.ValueTimePicker, culture.Name);

    public static int? GetMinIntegerValue(this Dictionary<string, object> attributes, CultureInfo culture) =>
        attributes.GetIntegerAttributeValue(InputAttributes.MinValue, culture.Name);

    public static int? GetMaxIntegerValue(this Dictionary<string, object> attributes, CultureInfo culture) =>
        attributes.GetIntegerAttributeValue(InputAttributes.MaxValue, culture.Name);

    public static decimal? GetMinDecimalValue(this Dictionary<string, object> attributes, CultureInfo culture) =>
        attributes.GetDecimalAttributeValue(InputAttributes.MinValue, culture.Name);

    public static decimal? GetMaxDecimalValue(this Dictionary<string, object> attributes, CultureInfo culture) =>
        attributes.GetDecimalAttributeValue(InputAttributes.MaxValue, culture.Name);

    public static DateTime? GetMinDateTimeValue(this Dictionary<string, object> attributes, CultureInfo culture) =>
        attributes.GetDateExpressionAttributeValue(InputAttributes.MinValue, culture);

    public static DateTime? GetMaxDateTimeValue(this Dictionary<string, object> attributes, CultureInfo culture) =>
        attributes.GetDateExpressionAttributeValue(InputAttributes.MaxValue, culture);

    public static int? GetIntegerStepSize(this Dictionary<string, object> attributes, CultureInfo culture) =>
        attributes.GetIntegerAttributeValue(InputAttributes.StepSize, culture.Name);

    public static decimal? GetDecimalStepSize(this Dictionary<string, object> attributes, CultureInfo culture) =>
        attributes.GetDecimalAttributeValue(InputAttributes.StepSize, culture.Name);

    public static string GetFormat(this Dictionary<string, object> attributes, CultureInfo culture) =>
        attributes.GetStringAttributeValue(InputAttributes.Format, culture.Name);

    public static int? GetLineCount(this Dictionary<string, object> attributes, CultureInfo culture) =>
        attributes.GetIntegerAttributeValue(InputAttributes.LineCount, culture.Name);

    public static int? GetMaxLength(this Dictionary<string, object> attributes, CultureInfo culture) =>
        attributes.GetIntegerAttributeValue(InputAttributes.MaxLength, culture.Name);

    public static bool? GetCheck(this Dictionary<string, object> attributes, CultureInfo culture) =>
        attributes.GetBooleanAttributeValue(InputAttributes.Check, culture.Name);

    public static bool? GetValueHistory(this Dictionary<string, object> attributes, CultureInfo culture) =>
        attributes.GetBooleanAttributeValue(InputAttributes.ValueHistory, culture.Name);

    // no value picker type: the case field value-type pre defines the picker type

    #endregion

    #region Case Field Attachments

    public static AttachmentType? GetAttachment(this Dictionary<string, object> attributes, CultureInfo culture) =>
        attributes.GetEnumAttributeValue<AttachmentType>(InputAttributes.Attachment, culture.Name);

    public static string GetAttachmentExtensions(this Dictionary<string, object> attributes, CultureInfo culture) =>
        attributes.GetStringAttributeValue(InputAttributes.AttachmentExtensions, culture.Name);

    #endregion

    #region List

    public static bool HasList(this Dictionary<string, object> attributes, CultureInfo culture) =>
        attributes.HasAttribute(InputAttributes.List, culture.Name);

    public static List<object> GetList(this Dictionary<string, object> attributes, CultureInfo culture) =>
        GetListAttributeValue(attributes, InputAttributes.List, culture);

    public static List<T> GetList<T>(this Dictionary<string, object> attributes, CultureInfo culture) =>
        GetListAttributeValue<T>(attributes, InputAttributes.List, culture);

    public static List<object> GetListValues(this Dictionary<string, object> attributes, CultureInfo culture) =>
        GetListAttributeValue(attributes, InputAttributes.ListValues, culture);

    public static List<T> GetListValues<T>(this Dictionary<string, object> attributes, CultureInfo culture) =>
        GetListAttributeValue<T>(attributes, InputAttributes.ListValues, culture);

    //public static bool HasListSelection(this Dictionary<string, object> attributes, CultureInfo culture) =>
    //    attributes.HasAttribute(InputAttributes.ListSelection, culture);

    public static T GetListSelection<T>(this Dictionary<string, object> attributes, CultureInfo culture)
    {
        if (attributes.TryGetAttributeValue<object>(InputAttributes.ListSelection, out var selection, culture.Name))
        {
            try
            {
                var value = (T)Convert.ChangeType(selection, typeof(T));
                return value;
            }
            catch (Exception exception)
            {
                throw new PayrollException($"Invalid json list selection {selection}.", exception);
            }
        }
        return default;
    }

    private static List<T> GetListAttributeValue<T>(this Dictionary<string, object> attributes, string attribute,
        CultureInfo culture)
    {
        var typeList = new List<T>();
        var objectList = GetListAttributeValue(attributes, attribute, culture);
        if (objectList != null)
        {
            foreach (var item in objectList)
            {
                try
                {
                    var value = (T)Convert.ChangeType(item, typeof(T));
                    typeList.Add(value);
                }
                catch (Exception exception)
                {
                    throw new PayrollException($"Invalid json list value {item}.", exception);
                }
            }
        }
        return typeList;
    }

    private static List<object> GetListAttributeValue(this Dictionary<string, object> attributes, string attribute, CultureInfo culture)
    {
        var value = attributes.GetStringAttributeValue(attribute, culture.Name);
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        // json array
        try
        {
            var list = JsonSerializer.Deserialize<List<object>>(value);

            // decompose list item
            for (var i = 0; i < list.Count; i++)
            {
                if (list[i] is JsonElement jsonElement)
                {
                    list[i] = jsonElement.GetValue();
                }
            }

            return list;
        }
        catch (JsonException exception)
        {
            throw new PayrollException($"Invalid json list: {value}.", exception);
        }
    }

    #endregion

    #region Date

    private static DateTime? GetDateExpressionAttributeValue(this IDictionary<string, object> attributes,
        string name, CultureInfo culture)
    {
        var expression = attributes.GetStringAttributeValue(name, culture.Name);
        return string.IsNullOrWhiteSpace(expression) ? null : Date.Parse(expression, culture);
    }

    #endregion

}