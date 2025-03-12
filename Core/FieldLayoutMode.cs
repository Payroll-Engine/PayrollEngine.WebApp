namespace PayrollEngine.WebApp;

/// <summary>
/// Field layout mode
/// </summary>
public enum FieldLayoutMode
{
    /// <summary>
    /// Use start date, end date and value
    /// </summary>
    StartEndValue,

    /// <summary>
    /// Use start date and value
    /// </summary>
    StartValue,

    /// <summary>
    /// Use value only
    /// </summary>
    Value,

    /// <summary>
    /// Use value only in compact mode
    /// </summary>
    ValueCompact
}