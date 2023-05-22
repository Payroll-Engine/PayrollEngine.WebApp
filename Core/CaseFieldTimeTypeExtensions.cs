namespace PayrollEngine.WebApp;

public static class CaseValueTimeTypeExtensions
{
    /// <summary>
    /// Test if time type support a start moment
    /// </summary>
    /// <param name="timeType">The value type</param>
    /// <returns>True for time types with a start</returns>
    public static bool HasStart(this CaseFieldTimeType timeType) =>
        timeType != CaseFieldTimeType.Timeless;

    /// <summary>
    /// Test if time type support an end moment
    /// </summary>
    /// <param name="timeType">The value type</param>
    /// <returns>True for time types with an end</returns>
    public static bool HasEnd(this CaseFieldTimeType timeType) =>
        timeType is CaseFieldTimeType.Period or CaseFieldTimeType.ScaledPeriod;
}