using Microsoft.Extensions.Localization;

namespace PayrollEngine.WebApp.Shared;

public class CalendarLocalizer : LocalizerBase
{
    public CalendarLocalizer(IStringLocalizerFactory factory) :
        base(factory)
    {
    }

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
}