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

    extension(Dictionary<string, object> attributes)
    {
        public Dictionary<string, object> GetEditInfo()
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

        public bool? GetValidity(CultureInfo culture) =>
            attributes.GetBooleanAttributeValue(InputAttributes.Validity, culture.Name);
    }

    #endregion

    #region Case General

    extension(Dictionary<string, object> attributes)
    {
        public string GetIcon(CultureInfo culture) =>
            attributes.GetStringAttributeValue(InputAttributes.Icon, culture.Name);

        public CasePriority? GetPriority(CultureInfo culture) =>
            attributes.GetEnumAttributeValue<CasePriority>(InputAttributes.Priority, culture.Name);
    }

    #endregion

    #region Case Field General

    extension(Dictionary<string, object> attributes)
    {
        public string GetGroup(CultureInfo culture) =>
            attributes.GetStringAttributeValue(InputAttributes.Group, culture.Name);

        public bool? GetSeparator(CultureInfo culture) =>
            attributes.GetBooleanAttributeValue(InputAttributes.Separator, culture.Name);

        public bool? GetHidden(CultureInfo culture) =>
            attributes.GetBooleanAttributeValue(InputAttributes.Hidden, culture.Name);

        public bool? GetHiddenName(CultureInfo culture) =>
            attributes.GetBooleanAttributeValue(InputAttributes.HiddenName, culture.Name);

        public FieldLayoutMode? GetFieldLayout(CultureInfo culture) =>
            attributes.GetEnumAttributeValue<FieldLayoutMode>(InputAttributes.FieldLayout, culture.Name);

        public bool? GetShowDescription(CultureInfo culture) =>
            attributes.GetBooleanAttributeValue(InputAttributes.ShowDescription, culture.Name);

        public InputVariant? GetVariant(CultureInfo culture) =>
            attributes.GetEnumAttributeValue<InputVariant>(InputAttributes.Variant, culture.Name);
    }

    #endregion

    #region Case Field Start

    extension(Dictionary<string, object> attributes)
    {
        public bool? GetStartReadOnly(CultureInfo culture) =>
            attributes.GetBooleanAttributeValue(InputAttributes.StartReadOnly, culture.Name);

        public bool? GetStartHidden(CultureInfo culture) =>
            attributes.GetBooleanAttributeValue(InputAttributes.StartHidden, culture.Name);

        public DatePickerType? GetStartPickerOpen(CultureInfo culture) =>
            attributes.GetEnumAttributeValue<DatePickerType>(InputAttributes.StartPickerOpen, culture.Name);

        public DateTimePickerType? GetStartPickerType(CultureInfo culture) =>
            attributes.GetEnumAttributeValue<DateTimePickerType>(InputAttributes.StartPickerType, culture.Name);

        public string GetStartFormat(CultureInfo culture)
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

        public string GetStartLabel(CultureInfo culture) =>
            attributes.GetStringAttributeValue(InputAttributes.StartLabel, culture.Name);

        public string GetStartHelp(CultureInfo culture) =>
            attributes.GetStringAttributeValue(InputAttributes.StartHelp, culture.Name);

        public string GetStartRequired(CultureInfo culture) =>
            attributes.GetStringAttributeValue(InputAttributes.StartRequired, culture.Name);
    }

    #endregion

    #region Case Field End

    extension(Dictionary<string, object> attributes)
    {
        public bool? GetEndReadOnly(CultureInfo culture) =>
            attributes.GetBooleanAttributeValue(InputAttributes.EndReadOnly, culture.Name);

        public bool? GetEndHidden(CultureInfo culture) =>
            attributes.GetBooleanAttributeValue(InputAttributes.EndHidden, culture.Name);

        public DatePickerType? GetEndPickerOpen(CultureInfo culture) =>
            attributes.GetEnumAttributeValue<DatePickerType>(InputAttributes.EndPickerOpen, culture.Name);

        public DateTimePickerType? GetEndPickerType(CultureInfo culture) =>
            attributes.GetEnumAttributeValue<DateTimePickerType>(InputAttributes.EndPickerType, culture.Name);

        public string GetEndFormat(CultureInfo culture)
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

        public string GetEndLabel(CultureInfo culture) =>
            attributes.GetStringAttributeValue(InputAttributes.EndLabel, culture.Name);

        public string GetEndHelp(CultureInfo culture) =>
            attributes.GetStringAttributeValue(InputAttributes.EndHelp, culture.Name);

        public string GetEndRequired(CultureInfo culture) =>
            attributes.GetStringAttributeValue(InputAttributes.EndRequired, culture.Name);
    }

    #endregion

    #region Case Field Value

    extension(Dictionary<string, object> attributes)
    {
        public string GetValueLabel(CultureInfo culture) =>
            attributes.GetStringAttributeValue(InputAttributes.ValueLabel, culture.Name);

        public string GetValueAdornment(CultureInfo culture) =>
            attributes.GetStringAttributeValue(InputAttributes.ValueAdornment, culture.Name);

        public string GetValueHelp(CultureInfo culture) =>
            attributes.GetStringAttributeValue(InputAttributes.ValueHelp, culture.Name);

        public string GetValueMask(CultureInfo culture) =>
            attributes.GetStringAttributeValue(InputAttributes.ValueMask, culture.Name);

        public string GetValueRequired(CultureInfo culture) =>
            attributes.GetStringAttributeValue(InputAttributes.ValueRequired, culture.Name);

        public bool? GetValueReadOnly(CultureInfo culture) =>
            attributes.GetBooleanAttributeValue(InputAttributes.ValueReadOnly, culture.Name);

        public DatePickerType? GetValuePickerOpen(CultureInfo culture) =>
            attributes.GetEnumAttributeValue<DatePickerType>(InputAttributes.ValuePickerOpen, culture.Name);

        public bool? GetValuePickerStatic(CultureInfo culture) =>
            attributes.GetBooleanAttributeValue(InputAttributes.ValuePickerStatic, culture.Name);

        public TimePickerType? GetValueTimePicker(CultureInfo culture) =>
            attributes.GetEnumAttributeValue<TimePickerType>(InputAttributes.ValueTimePicker, culture.Name);

        public int? GetMinIntegerValue(CultureInfo culture) =>
            attributes.GetIntegerAttributeValue(InputAttributes.MinValue, culture.Name);

        public int? GetMaxIntegerValue(CultureInfo culture) =>
            attributes.GetIntegerAttributeValue(InputAttributes.MaxValue, culture.Name);

        public decimal? GetMinDecimalValue(CultureInfo culture) =>
            attributes.GetDecimalAttributeValue(InputAttributes.MinValue, culture.Name);

        public decimal? GetMaxDecimalValue(CultureInfo culture) =>
            attributes.GetDecimalAttributeValue(InputAttributes.MaxValue, culture.Name);

        public DateTime? GetMinDateTimeValue(CultureInfo culture) =>
            attributes.GetDateExpressionAttributeValue(InputAttributes.MinValue, culture);

        public DateTime? GetMaxDateTimeValue(CultureInfo culture) =>
            attributes.GetDateExpressionAttributeValue(InputAttributes.MaxValue, culture);

        public int? GetIntegerStepSize(CultureInfo culture) =>
            attributes.GetIntegerAttributeValue(InputAttributes.StepSize, culture.Name);

        public decimal? GetDecimalStepSize(CultureInfo culture) =>
            attributes.GetDecimalAttributeValue(InputAttributes.StepSize, culture.Name);

        public string GetFormat(CultureInfo culture) =>
            attributes.GetStringAttributeValue(InputAttributes.Format, culture.Name);

        public int? GetLineCount(CultureInfo culture) =>
            attributes.GetIntegerAttributeValue(InputAttributes.LineCount, culture.Name);

        public int? GetMaxLength(CultureInfo culture) =>
            attributes.GetIntegerAttributeValue(InputAttributes.MaxLength, culture.Name);

        public bool? GetCheck(CultureInfo culture) =>
            attributes.GetBooleanAttributeValue(InputAttributes.Check, culture.Name);

        public bool? GetValueHistory(CultureInfo culture) =>
            attributes.GetBooleanAttributeValue(InputAttributes.ValueHistory, culture.Name);
    }

    // no value picker type: the case field value-type pre defines the picker type

    #endregion

    #region Case Field Attachments

    extension(Dictionary<string, object> attributes)
    {
        public AttachmentType? GetAttachment(CultureInfo culture) =>
            attributes.GetEnumAttributeValue<AttachmentType>(InputAttributes.Attachment, culture.Name);

        public string GetAttachmentExtensions(CultureInfo culture) =>
            attributes.GetStringAttributeValue(InputAttributes.AttachmentExtensions, culture.Name);
    }

    #endregion

    #region List

    extension(Dictionary<string, object> attributes)
    {
        public bool HasList(CultureInfo culture) =>
            attributes.HasAttribute(InputAttributes.List, culture.Name);

        public List<object> GetList(CultureInfo culture) => attributes.GetListAttributeValue(InputAttributes.List, culture);

        public List<T> GetList<T>(CultureInfo culture) => attributes.GetListAttributeValue<T>(InputAttributes.List, culture);

        public List<object> GetListValues(CultureInfo culture) => attributes.GetListAttributeValue(InputAttributes.ListValues, culture);

        public List<T> GetListValues<T>(CultureInfo culture) => attributes.GetListAttributeValue<T>(InputAttributes.ListValues, culture);

        public T GetListSelection<T>(CultureInfo culture)
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

        private List<T> GetListAttributeValue<T>(string attribute,
            CultureInfo culture)
        {
            var typeList = new List<T>();
            var objectList = attributes.GetListAttributeValue(attribute, culture);
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

        private List<object> GetListAttributeValue(string attribute, CultureInfo culture)
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
    }

    //public static bool HasListSelection(this Dictionary<string, object> attributes, CultureInfo culture) =>
    //    attributes.HasAttribute(InputAttributes.ListSelection, culture);

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