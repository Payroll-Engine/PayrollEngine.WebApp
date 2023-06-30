using System;
using System.Globalization;

namespace PayrollEngine.WebApp;

public static class Date
{
    #region Value

    /// <summary>
    /// Represents the smallest possible value of a time instant
    /// </summary>
    public static readonly DateTime MinValue = DateTime.MinValue.ToUtc();

    /// <summary>
    /// Represents the largest possible value of a time instant
    /// </summary>
    public static readonly DateTime MaxValue = DateTime.MaxValue.ToUtc();

    /// <summary>
    /// Gets a time instant that is set to the current date and time
    /// </summary>
    public static readonly DateTime Now = DateTime.UtcNow;

    /// <summary>
    /// Gets a time instant that is set to the current day
    /// </summary>
    private static readonly DateTime Today = DateTime.UtcNow.Date;

    /// <summary>
    /// Gets a time instant that is set to the next day
    /// </summary>
    private static readonly DateTime Tomorrow = Today.AddDays(1);

    /// <summary>
    /// Gets a time instant that is set to the previous day
    /// </summary>
    private static readonly DateTime Yesterday = Today.AddDays(-1);

    /// <summary>
    /// Get the previous tick
    /// </summary>
    /// <param name="dateTime">The source date time</param>
    /// <returns>The previous tick</returns>
    public static DateTime PreviousTick(DateTime dateTime) =>
        dateTime.AddTicks(-1);

    /// <summary>
    /// Get the next tick
    /// </summary>
    /// <param name="dateTime">The source date time</param>
    /// <returns>The next tick</returns>
    public static DateTime NextTick(DateTime dateTime) =>
        dateTime.AddTicks(1);

    /// <summary>
    /// Test if the date is midnight
    /// </summary>
    /// <param name="moment">The moment to test</param>
    /// <returns>True in case the moment is date</returns>
    private static bool IsMidnight(DateTime moment) =>
        // https://stackoverflow.com/questions/681435/what-is-the-best-way-to-determine-if-a-system-datetime-is-midnight
        moment.TimeOfDay.Ticks == 0;

    #endregion

    #region Calendar

    /// <summary>
    /// First month in year
    /// </summary>
    private static readonly int FirstMonthOfCalendarYear = 1;

    /// <summary>
    /// First day in month
    /// </summary>
    private static readonly int FirstDayOfMonth = 1;

    /// <summary>
    /// Number of months in a year
    /// </summary>
    private static readonly int MonthsInYear = 12;

    /// <summary>
    /// Last month in year
    /// </summary>
    private static readonly int LastMonthOfCalendarYear = MonthsInYear;

    /// <summary>
    /// Number of days in a week
    /// </summary>
    private static readonly int DaysInWeek = 7;

    /// <summary>
    /// Minimum calendar date
    /// </summary>
    public static readonly DateTime MinCalendarDate = new(1900, 1, 1);

    /// <summary>
    /// Maximum calendar date
    /// </summary>
    public static readonly DateTime MaxCalendarDate = new(2099, 12, 31);

    /// <summary>Returns the number of days in the specified month and year</summary>
    /// <param name="year">The year</param>
    /// <param name="month">The month (a number ranging from 1 to 12)</param>
    /// <returns>The number of days in <paramref name="month" /> for the specified <paramref name="year" />.
    /// For example, if <paramref name="month" /> equals 2 for February, the return value is 28 or 29 depending upon whether <paramref name="year" /> is a leap year.</returns>
    /// <exception cref="T:System.ArgumentOutOfRangeException">
    ///         <paramref name="month" /> is less than 1 or greater than 12
    /// -or-
    /// <paramref name="year" /> is less than 1 or greater than 9999.</exception>
    private static int DaysInMonth(int year, int month) =>
        DateTime.DaysInMonth(year, month);

    /// <summary>Returns the number of days in the specified month and year</summary>
    /// <param name="date">The date</param>
    /// <returns>The number of days in for the specified <paramref name="date" /></returns>
    private static int DaysInMonth(DateTime date) =>
        DateTime.DaysInMonth(date.Year, date.Month);

    #endregion

    #region Periods

    /// <summary>
    /// Test if a specific time moment is within a period
    /// </summary>
    /// <param name="start">The period start</param>
    /// <param name="end">The period end</param>
    /// <param name="test">The moment to test</param>
    /// <returns>True if the test is within start and end</returns>
    public static bool IsWithin(DateTime start, DateTime end, DateTime test) =>
        start < end ?
            test >= start && test <= end :
            test >= end && test <= start;

    /// <summary>
    /// Round the last moment to the next unit
    /// </summary>
    /// <param name="moment">Moment to round</param>
    public static DateTime RoundLastMoment(DateTime moment) =>
        IsLastMomentOfDay(moment) ? moment.Date.AddDays(1) : moment;

    #endregion

    #region Year Periods

    /// <summary>
    /// Return the first moment of a year
    /// </summary>
    /// <param name="year">The moment year</param>
    /// <returns><seealso cref="DateTime"/> from the first moment in a year</returns>
    public static DateTime FirstMomentOfYear(int year) =>
        new(year, FirstMonthOfCalendarYear, FirstDayOfMonth);

    /// <summary>
    /// Test if the date is the first moment of the year
    /// </summary>
    /// <param name="moment">Moment to test</param>
    /// <returns>True on the last moment of the year</returns>
    public static bool IsFirstMomentOfYear(DateTime moment) =>
        moment.Month == FirstMonthOfCalendarYear &&
        moment.Day == FirstDayOfMonth &&
        IsMidnight(moment);

    /// <summary>
    /// Return the last moment of a year
    /// </summary>
    /// <param name="year">The moment year</param>
    /// <returns><seealso cref="DateTime"/> from the latest moment in a month</returns>
    public static DateTime LastMomentOfYear(int year) =>
        LastMomentOfDay(new(year, LastMonthOfCalendarYear, DaysInMonth(year, LastMonthOfCalendarYear)));

    /// <summary>
    /// Test if the date is the last moment of the year
    /// </summary>
    /// <param name="moment">Moment to test</param>
    /// <returns>True on the last moment of the year</returns>
    public static bool IsLastMomentOfYear(DateTime moment) =>
        moment.Month == LastMonthOfCalendarYear &&
        moment.Day == DaysInMonth(moment) &&
        IsLastMomentOfDay(moment);

    #endregion

    #region Month Periods

    /// <summary>
    /// Return the month name of the current culture
    /// </summary>
    /// <param name="month">The month</param>
    /// <returns>Name of the month</returns>
    public static string GetMonthName(int month) =>
        GetMonthName(month, CultureInfo.CurrentCulture);

    /// <summary>
    /// Return the month name
    /// </summary>
    /// <param name="month">The month</param>
    /// <param name="culture">The culture</param>
    /// <returns>Name of the month</returns>
    private static string GetMonthName(int month, CultureInfo culture) =>
        culture.DateTimeFormat.GetMonthName(month);

    /// <summary>
    /// Return the first moment of a month
    /// </summary>
    /// <param name="year">The moment year</param>
    /// <param name="month">The moment month</param>
    /// <returns><seealso cref="DateTime"/> from the first moment in a month</returns>
    public static DateTime FirstMomentOfMonth(int year, int month) =>
        new(year, month, FirstDayOfMonth);

    /// <summary>
    /// Test if the date is the first moment of the month
    /// </summary>
    /// <param name="moment">Moment to test</param>
    /// <returns>True on the last moment of the month</returns>
    public static bool IsFirstMomentOfMonth(DateTime moment) =>
        moment.Day == FirstDayOfMonth &&
        IsMidnight(moment);

    /// <summary>
    /// Return the last moment of a month
    /// </summary>
    /// <param name="year">The moment year</param>
    /// <param name="month">The moment month</param>
    /// <returns><seealso cref="DateTime"/> from the latest moment in a month</returns>
    public static DateTime LastMomentOfMonth(int year, int month) =>
        LastMomentOfDay(new(year, month, DaysInMonth(year, month)));

    /// <summary>
    /// Test if the date is the last moment of the month
    /// </summary>
    /// <param name="moment">Moment to test</param>
    /// <returns>True on the last moment of the month</returns>
    public static bool IsLastMomentOfMonth(DateTime moment) =>
        moment.Day == DaysInMonth(moment) &&
        IsLastMomentOfDay(moment);

    /// <summary>
    /// Returns all month names
    /// </summary>
    public static string[] GetMonthNames => DateTimeFormatInfo.CurrentInfo.MonthNames;

    #endregion

    #region Day Periods

    /// <summary>
    /// Return the first moment of the day
    /// </summary>
    /// <param name="moment">Moment within the day</param>
    /// <returns><seealso cref="DateTime"/> from the first moment in a day</returns>
    public static DateTime FirstMomentOfDay(DateTime moment) =>
        moment.Date;

    /// <summary>
    /// Test if the date is the first moment of the day
    /// </summary>
    /// <param name="moment">Moment to test</param>
    /// <returns>True on the last moment of the day</returns>
    public static bool IsFirstMomentOfDay(DateTime moment) =>
        IsMidnight(moment);

    /// <summary>
    /// Return the last moment of the day
    /// </summary>
    /// <param name="moment">Moment within the day</param>
    /// <returns><seealso cref="DateTime"/> from the latest moment in a day</returns>
    private static DateTime LastMomentOfDay(DateTime moment) =>
        moment.Date.AddTicks(TimeSpan.TicksPerDay).PreviousTick();

    /// <summary>
    /// Test if the date is the last moment of the day
    /// Compare the day of the next tick with the current day
    /// </summary>
    /// <param name="moment">Moment to test</param>
    /// <returns>True on the last moment of the day</returns>
    private static bool IsLastMomentOfDay(DateTime moment) =>
        Equals(LastMomentOfDay(moment), moment);

    #endregion

    #region Parser

    public static DateTime? Parse(string expression)
    {
        if (string.IsNullOrWhiteSpace(expression))
        {
            return null;
        }

        // predefined expressions
        switch (expression.ToLower())
        {
            case "yesterday":
                return Yesterday;
            case "today":
                return Today;
            case "tomorrow":
                return Tomorrow;
            case "previousmonth":
                var previousMonth = Today.AddMonths(-1);
                return new(previousMonth.Year, previousMonth.Month, 1);
            case "month":
                var month = Today;
                return new(month.Year, month.Month, 1);
            case "nextmonth":
                var nextMonth = Today.AddMonths(1);
                return new(nextMonth.Year, nextMonth.Month, 1);
            case "previousyear":
                return new(Today.AddYears(-1).Year, 1, 1);
            case "year":
                return new(Today.Year, 1, 1);
            case "nextyear":
                return new(Today.AddYears(1).Year, 1, 1);
        }

        // offset
        if (expression.StartsWith("offset:", StringComparison.InvariantCultureIgnoreCase))
        {
            var offset = expression.Substring("offset:".Length);
            if (!string.IsNullOrWhiteSpace(offset))
            {
                var valueText = offset.Substring(0, offset.Length - 1).TrimStart('+');
                if (int.TryParse(valueText, out var value))
                {

                    switch (offset[^1])
                    {
                        case 'd':
                            return Today.AddDays(value);
                        case 'w':
                            return Today.AddDays(DaysInWeek * value);
                        case 'm':
                            return Today.AddMonths(value);
                        case 'y':
                            return Today.AddYears(value);
                    }
                }
            }
        }

        // date time parsing
        return ValueConvert.ToDateTime(expression);
    }

    #endregion

    #region Conversion

    /// <summary>
    /// Convert a date into the UTC value without UTC time adaption
    /// Dates (without time part) are used without time adaption
    /// </summary>
    /// <param name="moment">The source date time</param>
    /// <returns>The UTC date time</returns>
    public static DateTime SpecifyUtcTime(DateTime moment)
    {
        switch (moment.Kind)
        {
            case DateTimeKind.Utc:
                // already utc
                return moment;
            case DateTimeKind.Unspecified:
            case DateTimeKind.Local:
                // specify kind
                return DateTime.SpecifyKind(moment, DateTimeKind.Utc);
            default:
                throw new ArgumentOutOfRangeException(nameof(moment));
        }
    }

    #endregion
}