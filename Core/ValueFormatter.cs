﻿using System;
using System.Globalization;

namespace PayrollEngine.WebApp;

/// <summary>
/// Value formatter
/// </summary>
/// <param name="culture"></param>
public class ValueFormatter(CultureInfo culture) : IValueFormatter
{
    private CultureInfo Culture { get; } = culture ?? throw new ArgumentNullException(nameof(culture));

    /// <inheritdoc />
    public string ToString(string json, ValueType valueType, CultureInfo culture) =>
        string.IsNullOrWhiteSpace(json) ? null
            : ToString(ValueConvert.ToValue(json, valueType, culture), valueType, culture);

    /// <inheritdoc />
    public string ToString(object value, ValueType valueType, CultureInfo culture)
    {
        if (culture == null)
        {
            throw new ArgumentNullException(nameof(culture));
        }

        if (value == null)
        {
            return null;
        }

        var stringValue = value.ToString();

        switch (valueType)
        {
            // string
            case ValueType.String:
            case ValueType.WebResource:
                break;
            // date and time
            case ValueType.Date:
                stringValue = ((DateTime)value).ToString(culture.DateTimeFormat.ShortDatePattern, culture);
                break;
            case ValueType.DateTime:
                stringValue = ((DateTime)value).ToString(culture.DateTimeFormat.ShortDatePattern, culture) + " " +
                              ((DateTime)value).ToString(culture.DateTimeFormat.ShortTimePattern, culture);
                break;
            // numeric
            case ValueType.Integer:
            case ValueType.Year:
                stringValue = ((int)value).ToString("D", culture);
                break;
            case ValueType.Weekday:
                if (value is int weekday && Enum.IsDefined(typeof(DayOfWeek), weekday))
                {
                    stringValue = culture.DateTimeFormat.DayNames[weekday];
                }
                break;
            case ValueType.Month:
                if (value is int month && Enum.IsDefined(typeof(Month), month))
                {
                    stringValue = culture.DateTimeFormat.MonthNames[month - 1];
                }
                break;
            case ValueType.Decimal:
                stringValue = ((decimal)value).ToString("0.##", culture);
                break;
            case ValueType.Money:
                stringValue = ((decimal)value).ToString("C", culture);
                break;
            case ValueType.Percent:
                stringValue = ((decimal)value).ToString("0.##%", culture);
                break;
            case ValueType.NumericBoolean:
                stringValue = (decimal)value != 0 ? bool.TrueString : bool.FalseString;
                break;
            // boolean
            case ValueType.Boolean:
                stringValue = (bool)value ? bool.TrueString : bool.FalseString;
                break;
            case ValueType.None:
                // nothing to do
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(valueType), valueType, null);
        }

        return stringValue;
    }

    /// <inheritdoc />
    public string ToCompactString(DateTime? date)
    {
        if (!date.HasValue)
        {
            return null;
        }
        var culture = Culture;
        return date.Value.IsMidnight() ?
            date.Value.ToString(culture.DateTimeFormat.ShortDatePattern, culture) :
            date.Value.ToString(culture.DateTimeFormat.ShortDatePattern, culture) + " " +
                date.Value.ToString(culture.DateTimeFormat.ShortTimePattern, culture);
    }
}