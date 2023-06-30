using Microsoft.Extensions.Localization;

namespace PayrollEngine.WebApp.Shared;

public class CalendarLocalizer : LocalizerBase
{
    public CalendarLocalizer(IStringLocalizerFactory factory) :
        base(factory)
    {
    }

    public string Calendar => FromCaller();
    public string Calendars => FromCaller();
    public string NotAvailable => FromCaller();

    public string CycleTimeUnit => FromCaller();
    public string PeriodTimeUnit => FromCaller();
    public string TimeMap => FromCaller();
    public string WeekMode => FromCaller();

    public string Year => FromCaller();
    public string Month => FromCaller();
    public string Day => FromCaller();

    public string FirstMonthOfYear => FromCaller();
    public string MonthDayCount => FromCaller();
    public string YearWeekRule => FromCaller();
    public string FirstDayOfWeek => FromCaller();

    public string PeriodTimeUnitHelp => FromCaller();

    public string InvalidPeriodTimeUnit(CalendarTimeUnit cycle, CalendarTimeUnit period) =>
        string.Format(FromCaller(), FromEnum(cycle), FromEnum(period));
}