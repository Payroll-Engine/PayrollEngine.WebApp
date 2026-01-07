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

    /// <param name="dateTime">The source date time</param>
    extension(DateTime dateTime)
    {
        /// <summary>
        /// Convert a date into the UTC value with UTC time adaption
        /// </summary>
        /// <returns>The UTC date time</returns>
        public DateTime SpecifyUtc() =>
            Date.SpecifyUtcTime(dateTime);

        /// <summary>
        /// Convert a date into the OData date time format
        /// see https://stackoverflow.com/a/7400007/15659039
        /// </summary>
        /// <returns>The UTC date time string</returns>
        public string ToODataString() =>
            dateTime.ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture);
    }
}