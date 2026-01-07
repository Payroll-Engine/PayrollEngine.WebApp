namespace PayrollEngine.WebApp;

/// <summary>
/// Extensions for <see cref="CaseFieldTimeType" />
/// </summary>
public static class CaseValueTimeTypeExtensions
{
    /// <param name="timeType">The value type</param>
    extension(CaseFieldTimeType timeType)
    {
        /// <summary>
        /// Test if time type support a start moment
        /// </summary>
        /// <returns>True for time types with a start</returns>
        public bool HasStart() =>
            timeType != CaseFieldTimeType.Timeless;

        /// <summary>
        /// Test if time type support an end moment
        /// </summary>
        /// <returns>True for time types with an end</returns>
        public bool HasEnd() =>
            timeType is CaseFieldTimeType.Period or CaseFieldTimeType.CalendarPeriod;
    }
}