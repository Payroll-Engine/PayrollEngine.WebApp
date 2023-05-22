using System;
using System.Globalization;

namespace PayrollEngine.WebApp;

/// <summary>
/// Extensions for <see cref="DateTime"/>
/// </summary>
public static class DateTimeExtensions
{
    /// <summary>
    /// Test if the date is undefined
    /// </summary>
    /// <param name="dateTime">The date time to test</param>
    /// <returns>True, if the date time is undefined</returns>
    public static bool IsUndefined(this DateTime dateTime) =>
        dateTime == Date.MinValue;

    /// <summary>
    /// Test if the date is defined
    /// </summary>
    /// <param name="dateTime">The date time to test</param>
    /// <returns>True, if the date time is defined</returns>
    public static bool IsDefined(this DateTime dateTime) =>
        !IsUndefined(dateTime);

    /// <summary>
    /// Test if the date is midnight
    /// </summary>
    /// <param name="moment">The moment to test</param>
    /// <returns>True in case the moment is date</returns>
    public static bool IsMidnight(this DateTime moment) =>
        Date.IsMidnight(moment);

    /// <summary>
    /// Test if a specific time moment is within a date period
    /// </summary>
    /// <param name="moment">The moment to test</param>
    /// <param name="start">The period start</param>
    /// <param name="end">The period end</param>
    /// <returns>True if the moment is within the start end end date</returns>
    public static bool IsWithin(this DateTime moment, DateTime start, DateTime end) =>
        Date.IsWithin(start, end, moment);

    /// <summary>
    /// Test if date contains a time part
    /// </summary>
    /// <param name="moment">The date to test</param>
    /// <returns>True, if time part is present</returns>
    public static bool HasTime(this DateTime? moment)
    {
        if (moment == null)
        {
            // can't be true if date is null
            return false;
        }
        return moment.Value.TimeOfDay != TimeSpan.Zero;
    }

    /// <summary>
    /// Convert a date into the UTC value with UTC time adaption
    /// </summary>
    /// <param name="dateTime">The source date time</param>
    /// <returns>The UTC date time</returns>
    public static DateTime SpecifyUtc(this DateTime dateTime) =>
        Date.SpecifyUtcTime(dateTime);

    /// <summary>
    /// Convert a date into the OData date time format
    /// see https://stackoverflow.com/a/7400007/15659039
    /// </summary>
    /// <param name="dateTime">The date time</param>
    /// <returns>The UTC date time string</returns>
    public static string ToODataString(this DateTime dateTime) =>
        dateTime.ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture);

    /// <summary>
    /// Get the previous tick
    /// </summary>
    /// <param name="dateTime">The source date time</param>
    /// <returns>The previous tick</returns>
    public static DateTime PreviousTick(this DateTime dateTime) =>
        Date.PreviousTick(dateTime);

    /// <summary>
    /// Get the next tick
    /// </summary>
    /// <param name="dateTime">The source date time</param>
    /// <returns>The next tick</returns>
    public static DateTime NextTick(this DateTime dateTime) =>
        Date.NextTick(dateTime);

    /// <summary>
    /// Round the last moment to the next unit
    /// </summary>
    /// <param name="moment">Moment to round</param>
    public static DateTime RoundLastMoment(this DateTime moment) =>
        Date.RoundLastMoment(moment);

    /// <summary>
    /// Return the first moment of the day
    /// </summary>
    /// <param name="dateTime">The date time</param>
    /// <returns><seealso cref="System.DateTime"/> from the first moment in a day</returns>
    public static DateTime FirstMomentOfDay(this DateTime dateTime) =>
        Date.FirstMomentOfDay(dateTime);

    /// <summary>
    /// Test if the date is the first moment of the day
    /// </summary>
    /// <param name="dateTime">The date time</param>
    /// <returns>True on the last moment of the day</returns>
    public static bool IsFirstMomentOfDay(this DateTime dateTime) =>
        Date.IsFirstMomentOfDay(dateTime);

    /// <summary>
    /// Return the last moment of the day
    /// </summary>
    /// <param name="dateTime">The date time</param>
    /// <returns><seealso cref="System.DateTime"/> from the latest moment in a day</returns>
    public static DateTime LastMomentOfDay(this DateTime dateTime) =>
        Date.LastMomentOfDay(dateTime);

    /// <summary>
    /// Test if the date is the last moment of the day
    /// Compare the day of the next tick with the current day
    /// </summary>
    /// <param name="dateTime">The date time</param>
    /// <returns>True on the last moment of the day</returns>
    public static bool IsLastMomentOfDay(this DateTime dateTime) =>
        Date.IsLastMomentOfDay(dateTime);

    /// <summary>
    /// Test if date is the first day of year
    /// </summary>
    /// <param name="date">The date to test</param>
    /// <returns>Return true if the date is in the first dya of the year</returns>
    public static bool IsFirstDayOfCalendarYear(this DateTime date) =>
        date.Month == Date.FirstMonthOfCalendarYear && IsFirstDayOfMonth(date);

    /// <summary>
    /// Test if date is the last day of year
    /// </summary>
    /// <param name="date">The date to test</param>
    /// <returns>Return true if the date is in the first dya of the year</returns>
    public static bool IsLastDayOfCalendarYear(this DateTime date) =>
        date.Month == Date.LastMonthOfCalendarYear && IsLastDayOfMonth(date);

    /// <summary>
    /// Test if date is the first day of month
    /// </summary>
    /// <param name="date">The date to test</param>
    /// <returns>Return true if the date is in the first dya of the year</returns>
    public static bool IsFirstDayOfMonth(this DateTime date) =>
        date.Day == Date.FirstDayOfMonth;

    /// <summary>
    /// Test if date is the last day of month
    /// </summary>
    /// <param name="date">The date to test</param>
    /// <returns>Return true if the date is in the first dya of the year</returns>
    public static bool IsLastDayOfMonth(this DateTime date) =>
        date.Day == Date.DaysInMonth(date.Year, date.Month);

    /// <summary>
    /// Test if two dates are in the same year
    /// </summary>
    /// <param name="date">The first date to test</param>
    /// <param name="compare">The second date to test</param>
    /// <returns>Return true if year and mont of both dates is equal</returns>
    public static bool IsSameYear(this DateTime date, DateTime compare) =>
        date.Year == compare.Year;

    /// <summary>
    /// Test if two dates are in the same year and month
    /// </summary>
    /// <param name="date">The first date to test</param>
    /// <param name="compare">The second date to test</param>
    /// <returns>Return true if year and mont of both dates is equal</returns>
    public static bool IsSameMonth(this DateTime date, DateTime compare) =>
        date.IsSameYear(compare) && date.Month == compare.Month;
}