using System;
using System.Collections.Generic;
using System.Text.Json;

namespace PayrollEngine.WebApp;

public static class InputAttributesExtensions
{

    #region General

    public static bool? GetHidden(this Dictionary<string, object> attributes, Language? language = null) =>
        attributes.GetBooleanAttributeValue(InputAttributes.Hidden, language);

    public static bool? GetShowDescription(this Dictionary<string, object> attributes, Language? language = null) =>
        attributes.GetBooleanAttributeValue(InputAttributes.ShowDescription, language);

    public static int? GetSortOrder(this Dictionary<string, object> attributes, Language? language = null) =>
        attributes.GetIntegerAttributeValue(InputAttributes.SortOrder, language);

    #endregion

    #region Start

    public static bool? GetStartReadOnly(this Dictionary<string, object> attributes, Language? language = null) =>
        attributes.GetBooleanAttributeValue(InputAttributes.StartPickerOpen, language);

    public static DatePickerType? GetStartPickerOpen(this Dictionary<string, object> attributes, Language? language = null) =>
        attributes.GetEnumAttributeValue<DatePickerType>(InputAttributes.StartPickerOpen, language);

    public static DateTimeType? GetStartPickerType(this Dictionary<string, object> attributes, Language? language = null) =>
        attributes.GetEnumAttributeValue<DateTimeType>(InputAttributes.StartPickerType, language);

    public static string GetStartFormat(this Dictionary<string, object> attributes, Language? language = null)
    {
        if (!attributes.TryGetAttributeValue<object>(InputAttributes.StartFormat, out var value, language))
        {
            return null;
        }
        if (value is JsonElement jsonElement)
        {
            value = jsonElement.GetValue();
        }
        return value as string;
    }

    public static string GetStartLabel(this Dictionary<string, object> attributes, Language? language = null) =>
        attributes.GetStringAttributeValue(InputAttributes.StartLabel, language);

    public static string GetStartHelp(this Dictionary<string, object> attributes, Language? language = null) =>
        attributes.GetStringAttributeValue(InputAttributes.StartHelp, language);

    public static string GetStartRequired(this Dictionary<string, object> attributes, Language? language = null) =>
        attributes.GetStringAttributeValue(InputAttributes.StartRequired, language);

    #endregion

    #region End

    public static bool? GetEndReadOnly(this Dictionary<string, object> attributes, Language? language = null) =>
        attributes.GetBooleanAttributeValue(InputAttributes.EndPickerOpen, language);

    public static DatePickerType? GetEndPickerOpen(this Dictionary<string, object> attributes, Language? language = null) =>
        attributes.GetEnumAttributeValue<DatePickerType>(InputAttributes.EndPickerOpen, language);

    public static DateTimeType? GetEndPickerType(this Dictionary<string, object> attributes, Language? language = null) =>
        attributes.GetEnumAttributeValue<DateTimeType>(InputAttributes.EndPickerType, language);

    public static string GetEndFormat(this Dictionary<string, object> attributes, Language? language = null)
    {
        if (!attributes.TryGetAttributeValue<object>(InputAttributes.EndFormat, out var value, language))
        {
            return null;
        }
        if (value is JsonElement jsonElement)
        {
            value = jsonElement.GetValue();
        }
        return value as string;
    }

    public static string GetEndLabel(this Dictionary<string, object> attributes, Language? language = null) =>
        attributes.GetStringAttributeValue(InputAttributes.EndLabel, language);

    public static string GetEndHelp(this Dictionary<string, object> attributes, Language? language = null) =>
        attributes.GetStringAttributeValue(InputAttributes.EndHelp, language);

    public static string GetEndRequired(this Dictionary<string, object> attributes, Language? language = null) =>
        attributes.GetStringAttributeValue(InputAttributes.EndRequired, language);

    #endregion

    #region Value

    public static string GetValueLabel(this Dictionary<string, object> attributes, Language? language = null) =>
        attributes.GetStringAttributeValue(InputAttributes.ValueLabel, language);

    public static string GetValueAdornment(this Dictionary<string, object> attributes, Language? language = null) =>
        attributes.GetStringAttributeValue(InputAttributes.ValueAdornment, language);

    public static string GetValueHelp(this Dictionary<string, object> attributes, Language? language = null) =>
        attributes.GetStringAttributeValue(InputAttributes.ValueHelp, language);
    
    public static string GetValueMask(this Dictionary<string, object> attributes, Language? language = null) =>
        attributes.GetStringAttributeValue(InputAttributes.ValueMask, language);

    public static string GetValueRequired(this Dictionary<string, object> attributes, Language? language = null) =>
        attributes.GetStringAttributeValue(InputAttributes.ValueRequired, language);

    public static bool? GetValueReadOnly(this Dictionary<string, object> attributes, Language? language = null) =>
        attributes.GetBooleanAttributeValue(InputAttributes.ValueReadOnly, language);

    public static DatePickerType? GetValuePickerOpen(this Dictionary<string, object> attributes, Language? language = null) =>
        attributes.GetEnumAttributeValue<DatePickerType>(InputAttributes.ValuePickerOpen, language);

    public static string GetCulture(this Dictionary<string, object> attributes, Language? language = null) =>
        attributes.GetStringAttributeValue(InputAttributes.Culture, language);

    public static int? GetMinIntegerValue(this Dictionary<string, object> attributes, Language? language = null) =>
        attributes.GetIntegerAttributeValue(InputAttributes.MinValue, language);

    public static int? GetMaxIntegerValue(this Dictionary<string, object> attributes, Language? language = null) =>
        attributes.GetIntegerAttributeValue(InputAttributes.MaxValue, language);

    public static decimal? GetMinDecimalValue(this Dictionary<string, object> attributes, Language? language = null) =>
        attributes.GetDecimalAttributeValue(InputAttributes.MinValue, language);

    public static decimal? GetMaxDecimalValue(this Dictionary<string, object> attributes, Language? language = null) =>
        attributes.GetDecimalAttributeValue(InputAttributes.MaxValue, language);

    public static DateTime? GetMinDateTimeValue(this Dictionary<string, object> attributes, Language? language = null) =>
        attributes.GetDateExpressionAttributeValue(InputAttributes.MinValue, language);

    public static DateTime? GetMaxDateTimeValue(this Dictionary<string, object> attributes, Language? language = null) =>
        attributes.GetDateExpressionAttributeValue(InputAttributes.MaxValue, language);

    public static int? GetIntegerStepSize(this Dictionary<string, object> attributes, Language? language = null) =>
        attributes.GetIntegerAttributeValue(InputAttributes.StepSize, language);

    public static decimal? GetDecimalStepSize(this Dictionary<string, object> attributes, Language? language = null) =>
        attributes.GetDecimalAttributeValue(InputAttributes.StepSize, language);

    public static string GetFormat(this Dictionary<string, object> attributes, Language? language = null) =>
        attributes.GetStringAttributeValue(InputAttributes.Format, language);

    public static int? GetLineCount(this Dictionary<string, object> attributes, Language? language = null) =>
        attributes.GetIntegerAttributeValue(InputAttributes.LineCount, language);

    public static int? GetMaxLength(this Dictionary<string, object> attributes, Language? language = null) =>
        attributes.GetIntegerAttributeValue(InputAttributes.MaxLength, language);

    public static bool? GetCheck(this Dictionary<string, object> attributes, Language? language = null) =>
        attributes.GetBooleanAttributeValue(InputAttributes.Check, language);

    // no value picker type: the case field value-type pre defines the picker type

    #endregion

    #region Attachments

    public static AttachmentType? GetAttachment(this Dictionary<string, object> attributes, Language? language = null) =>
        attributes.GetEnumAttributeValue<AttachmentType>(InputAttributes.Attachment, language);

    public static string GetAttachmentExtensions(this Dictionary<string, object> attributes, Language? language = null) =>
        attributes.GetStringAttributeValue(InputAttributes.AttachmentExtensions, language);

    #endregion

    #region List

    public static bool HasList(this Dictionary<string, object> attributes, Language? language = null) =>
        attributes.HasAttribute(InputAttributes.List, language);

    public static List<object> GetList(this Dictionary<string, object> attributes, Language? language = null) =>
        GetListAttributeValue(attributes, InputAttributes.List, language);

    public static List<T> GetList<T>(this Dictionary<string, object> attributes, Language? language = null) =>
        GetListAttributeValue<T>(attributes, InputAttributes.List, language);

    public static List<object> GetListValues(this Dictionary<string, object> attributes, Language? language = null) =>
        GetListAttributeValue(attributes, InputAttributes.ListValues, language);

    public static List<T> GetListValues<T>(this Dictionary<string, object> attributes, Language? language = null) =>
        GetListAttributeValue<T>(attributes, InputAttributes.ListValues, language);

    //public static bool HasListSelection(this Dictionary<string, object> attributes, Language? language = null) =>
    //    attributes.HasAttribute(InputAttributes.ListSelection, language);

    public static T GetListSelection<T>(this Dictionary<string, object> attributes, Language? language = null)
    {
        if (attributes.TryGetAttributeValue<object>(InputAttributes.ListSelection, out var selection, language))
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
        Language? language = null)
    {
        var typeList = new List<T>();
        var objectList = GetListAttributeValue(attributes, attribute, language);
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

    private static List<object> GetListAttributeValue(this Dictionary<string, object> attributes, string attribute, Language? language = null)
    {
        var value = attributes.GetStringAttributeValue(attribute, language);
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

    public static DateTime? GetDateExpressionAttributeValue(this IDictionary<string, object> attributes,
        string name, Language? language = null)
    {
        var expression = attributes.GetStringAttributeValue(name, language);
        return string.IsNullOrWhiteSpace(expression) ? null : Date.Parse(expression);
    }

    #endregion

}