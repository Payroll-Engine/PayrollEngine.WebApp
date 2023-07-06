using System;
using System.Collections.Generic;
using System.Text.Json;

namespace PayrollEngine.WebApp;

public static class InputAttributesExtensions
{

    #region General

    public static bool? GetHidden(this Dictionary<string, object> attributes, string culture = null) =>
        attributes.GetBooleanAttributeValue(InputAttributes.Hidden, culture);

    public static bool? GetShowDescription(this Dictionary<string, object> attributes, string culture = null) =>
        attributes.GetBooleanAttributeValue(InputAttributes.ShowDescription, culture);

    #endregion

    #region Start

    public static bool? GetStartReadOnly(this Dictionary<string, object> attributes, string culture = null) =>
        attributes.GetBooleanAttributeValue(InputAttributes.StartPickerOpen, culture);

    public static DatePickerType? GetStartPickerOpen(this Dictionary<string, object> attributes, string culture = null) =>
        attributes.GetEnumAttributeValue<DatePickerType>(InputAttributes.StartPickerOpen, culture);

    public static DateTimeType? GetStartPickerType(this Dictionary<string, object> attributes, string culture = null) =>
        attributes.GetEnumAttributeValue<DateTimeType>(InputAttributes.StartPickerType, culture);

    public static string GetStartFormat(this Dictionary<string, object> attributes, string culture = null)
    {
        if (!attributes.TryGetAttributeValue<object>(InputAttributes.StartFormat, out var value, culture))
        {
            return null;
        }
        if (value is JsonElement jsonElement)
        {
            value = jsonElement.GetValue();
        }
        return value as string;
    }

    public static string GetStartLabel(this Dictionary<string, object> attributes, string culture = null) =>
        attributes.GetStringAttributeValue(InputAttributes.StartLabel, culture);

    public static string GetStartHelp(this Dictionary<string, object> attributes, string culture = null) =>
        attributes.GetStringAttributeValue(InputAttributes.StartHelp, culture);

    public static string GetStartRequired(this Dictionary<string, object> attributes, string culture = null) =>
        attributes.GetStringAttributeValue(InputAttributes.StartRequired, culture);

    #endregion

    #region End

    public static bool? GetEndReadOnly(this Dictionary<string, object> attributes, string culture = null) =>
        attributes.GetBooleanAttributeValue(InputAttributes.EndPickerOpen, culture);

    public static DatePickerType? GetEndPickerOpen(this Dictionary<string, object> attributes, string culture = null) =>
        attributes.GetEnumAttributeValue<DatePickerType>(InputAttributes.EndPickerOpen, culture);

    public static DateTimeType? GetEndPickerType(this Dictionary<string, object> attributes, string culture = null) =>
        attributes.GetEnumAttributeValue<DateTimeType>(InputAttributes.EndPickerType, culture);

    public static string GetEndFormat(this Dictionary<string, object> attributes, string culture = null)
    {
        if (!attributes.TryGetAttributeValue<object>(InputAttributes.EndFormat, out var value, culture))
        {
            return null;
        }
        if (value is JsonElement jsonElement)
        {
            value = jsonElement.GetValue();
        }
        return value as string;
    }

    public static string GetEndLabel(this Dictionary<string, object> attributes, string culture = null) =>
        attributes.GetStringAttributeValue(InputAttributes.EndLabel, culture);

    public static string GetEndHelp(this Dictionary<string, object> attributes, string culture = null) =>
        attributes.GetStringAttributeValue(InputAttributes.EndHelp, culture);

    public static string GetEndRequired(this Dictionary<string, object> attributes, string culture = null) =>
        attributes.GetStringAttributeValue(InputAttributes.EndRequired, culture);

    #endregion

    #region Value

    public static string GetValueLabel(this Dictionary<string, object> attributes, string culture = null) =>
        attributes.GetStringAttributeValue(InputAttributes.ValueLabel, culture);

    public static string GetValueAdornment(this Dictionary<string, object> attributes, string culture = null) =>
        attributes.GetStringAttributeValue(InputAttributes.ValueAdornment, culture);

    public static string GetValueHelp(this Dictionary<string, object> attributes, string culture = null) =>
        attributes.GetStringAttributeValue(InputAttributes.ValueHelp, culture);
    
    public static string GetValueMask(this Dictionary<string, object> attributes, string culture = null) =>
        attributes.GetStringAttributeValue(InputAttributes.ValueMask, culture);

    public static string GetValueRequired(this Dictionary<string, object> attributes, string culture = null) =>
        attributes.GetStringAttributeValue(InputAttributes.ValueRequired, culture);

    public static bool? GetValueReadOnly(this Dictionary<string, object> attributes, string culture = null) =>
        attributes.GetBooleanAttributeValue(InputAttributes.ValueReadOnly, culture);

    public static DatePickerType? GetValuePickerOpen(this Dictionary<string, object> attributes, string culture = null) =>
        attributes.GetEnumAttributeValue<DatePickerType>(InputAttributes.ValuePickerOpen, culture);

    public static string GetCulture(this Dictionary<string, object> attributes, string culture = null) =>
        attributes.GetStringAttributeValue(InputAttributes.Culture, culture);

    public static int? GetMinIntegerValue(this Dictionary<string, object> attributes, string culture = null) =>
        attributes.GetIntegerAttributeValue(InputAttributes.MinValue, culture);

    public static int? GetMaxIntegerValue(this Dictionary<string, object> attributes, string culture = null) =>
        attributes.GetIntegerAttributeValue(InputAttributes.MaxValue, culture);

    public static decimal? GetMinDecimalValue(this Dictionary<string, object> attributes, string culture = null) =>
        attributes.GetDecimalAttributeValue(InputAttributes.MinValue, culture);

    public static decimal? GetMaxDecimalValue(this Dictionary<string, object> attributes, string culture = null) =>
        attributes.GetDecimalAttributeValue(InputAttributes.MaxValue, culture);

    public static DateTime? GetMinDateTimeValue(this Dictionary<string, object> attributes, string culture = null) =>
        attributes.GetDateExpressionAttributeValue(InputAttributes.MinValue, culture);

    public static DateTime? GetMaxDateTimeValue(this Dictionary<string, object> attributes, string culture = null) =>
        attributes.GetDateExpressionAttributeValue(InputAttributes.MaxValue, culture);

    public static int? GetIntegerStepSize(this Dictionary<string, object> attributes, string culture = null) =>
        attributes.GetIntegerAttributeValue(InputAttributes.StepSize, culture);

    public static decimal? GetDecimalStepSize(this Dictionary<string, object> attributes, string culture = null) =>
        attributes.GetDecimalAttributeValue(InputAttributes.StepSize, culture);

    public static string GetFormat(this Dictionary<string, object> attributes, string culture = null) =>
        attributes.GetStringAttributeValue(InputAttributes.Format, culture);

    public static int? GetLineCount(this Dictionary<string, object> attributes, string culture = null) =>
        attributes.GetIntegerAttributeValue(InputAttributes.LineCount, culture);

    public static int? GetMaxLength(this Dictionary<string, object> attributes, string culture = null) =>
        attributes.GetIntegerAttributeValue(InputAttributes.MaxLength, culture);

    public static bool? GetCheck(this Dictionary<string, object> attributes, string culture = null) =>
        attributes.GetBooleanAttributeValue(InputAttributes.Check, culture);

    // no value picker type: the case field value-type pre defines the picker type

    #endregion

    #region Attachments

    public static AttachmentType? GetAttachment(this Dictionary<string, object> attributes, string culture = null) =>
        attributes.GetEnumAttributeValue<AttachmentType>(InputAttributes.Attachment, culture);

    public static string GetAttachmentExtensions(this Dictionary<string, object> attributes, string culture = null) =>
        attributes.GetStringAttributeValue(InputAttributes.AttachmentExtensions, culture);

    #endregion

    #region List

    public static bool HasList(this Dictionary<string, object> attributes, string culture = null) =>
        attributes.HasAttribute(InputAttributes.List, culture);

    public static List<object> GetList(this Dictionary<string, object> attributes, string culture = null) =>
        GetListAttributeValue(attributes, InputAttributes.List, culture);

    public static List<T> GetList<T>(this Dictionary<string, object> attributes, string culture = null) =>
        GetListAttributeValue<T>(attributes, InputAttributes.List, culture);

    public static List<object> GetListValues(this Dictionary<string, object> attributes, string culture = null) =>
        GetListAttributeValue(attributes, InputAttributes.ListValues, culture);

    public static List<T> GetListValues<T>(this Dictionary<string, object> attributes, string culture = null) =>
        GetListAttributeValue<T>(attributes, InputAttributes.ListValues, culture);

    //public static bool HasListSelection(this Dictionary<string, object> attributes, string culture = null) =>
    //    attributes.HasAttribute(InputAttributes.ListSelection, culture);

    public static T GetListSelection<T>(this Dictionary<string, object> attributes, string culture = null)
    {
        if (attributes.TryGetAttributeValue<object>(InputAttributes.ListSelection, out var selection, culture))
        {
            try
            {
                var value = (T)Convert.ChangeType(selection, typeof(T));
                return value;
            }
            catch (Exception exception)
            {
                throw new PayrollException($"Invalid json list selection {selection}", exception);
            }
        }
        return default;
    }

    private static List<T> GetListAttributeValue<T>(this Dictionary<string, object> attributes, string attribute,
        string culture = null)
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
                    throw new PayrollException($"Invalid json list value {item}", exception);
                }
            }
        }
        return typeList;
    }

    private static List<object> GetListAttributeValue(this Dictionary<string, object> attributes, string attribute, string culture = null)
    {
        var value = attributes.GetStringAttributeValue(attribute, culture);
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
            throw new PayrollException($"Invalid json list: {value}", exception);
        }
    }

    #endregion

    #region Date

    private static DateTime? GetDateExpressionAttributeValue(this IDictionary<string, object> attributes,
        string name, string culture = null)
    {
        var expression = attributes.GetStringAttributeValue(name, culture);
        return string.IsNullOrWhiteSpace(expression) ? null : Date.Parse(expression);
    }

    #endregion

}