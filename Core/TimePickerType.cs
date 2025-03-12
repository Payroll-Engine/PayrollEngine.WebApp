namespace PayrollEngine.WebApp;

/// <summary>
/// Time picker type
/// </summary>
public enum TimePickerType
{
    /// <summary>
    /// Hour and minute picker with 24 hours
    /// </summary>
    Day24,

    /// <summary>
    /// Hour and minute picker with am/pm
    /// </summary>
    Day12,

    /// <summary>
    /// Day hour picker (only hours)
    /// </summary>
    DayHour,

    /// <summary>
    /// Open to minute picker (max day)
    /// </summary>
    DayMinute,

    /// <summary>
    /// Hour minute picker (only minutes, max 60)
    /// </summary>
    HourMinute
}