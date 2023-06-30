using System;
using System.Globalization;

namespace PayrollEngine.WebApp;

/// <summary>
/// Extensions for <see cref="DateTime"/>
/// </summary>
public static class DateTimeExtensions
{
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

}