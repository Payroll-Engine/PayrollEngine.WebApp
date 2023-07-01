using System;
using System.Globalization;

namespace PayrollEngine.WebApp;

public class ValueFormatter : IValueFormatter
{
    private CultureInfo Culture { get; }

    public ValueFormatter(CultureInfo culture)
    {
        Culture = culture ?? throw new ArgumentNullException(nameof(culture));
    }

    /// <inheritdoc />
    public string ToString(string json, ValueType valueType) =>
        string.IsNullOrWhiteSpace(json) ? null
            : ToString(ValueConvert.ToValue(json, valueType), valueType);

    /// <inheritdoc />
    public string ToString(object value, ValueType valueType)
    {
        if (value == null)
        {
            return null;
        }

        var culture = Culture;
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
                stringValue = ((decimal)value).ToString("G", culture);
                break;
            case ValueType.Money:
                stringValue = ((decimal)value).ToString("C", culture);
                break;
            case ValueType.Percent:
                stringValue = ((decimal)value).ToString("P", culture);
                break;
            case ValueType.NumericBoolean:
                stringValue = (decimal)value != default ? bool.TrueString : bool.FalseString;
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