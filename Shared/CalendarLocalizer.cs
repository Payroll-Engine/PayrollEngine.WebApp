using Microsoft.Extensions.Localization;
using System.Globalization;

namespace PayrollEngine.WebApp.Shared;

public class CalendarLocalizer(IStringLocalizerFactory factory, CultureInfo culture) : 
    LocalizerBase(factory, culture: culture)
{
    public string Calendar => PropertyValue();
    public string Calendars => PropertyValue();
    public string NotAvailable => PropertyValue();

    public string CycleTimeUnit => PropertyValue();
    public string PeriodTimeUnit => PropertyValue();
    public string TimeMap => PropertyValue();
    public string WeekMode => PropertyValue();

    public string Year => PropertyValue();
    public string Month => PropertyValue();
    public string Day => PropertyValue();

    public string FirstMonthOfYear => PropertyValue();
    public string PeriodDayCount => PropertyValue();
    public string YearWeekRule => PropertyValue();
    public string FirstDayOfWeek => PropertyValue();

    public string PeriodTimeUnitHelp => PropertyValue();
    public string PeriodDayCountHelp => PropertyValue();

    public string InvalidPeriodTimeUnit(CalendarTimeUnit cycle, CalendarTimeUnit period) =>
        FormatValue(PropertyValue(), nameof(cycle), Enum(cycle), nameof(period), Enum(period));

    public string PeriodCycleHelp(CalendarTimeUnit cycle, CalendarTimeUnit period)
    {
        var value = PropertyValue();
        var count = cycle.PeriodCount(period);
        value = FormatValue(value, nameof(cycle), Enum(cycle));
        value = FormatValue(value, nameof(period), Enum(period));
        return FormatValue(value, nameof(count), count);
    }
}