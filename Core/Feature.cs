namespace PayrollEngine.WebApp;

/// <summary>
/// Application features
/// </summary>
public enum Feature
{
    #region Main

    /// <summary>
    /// Tasks feature
    /// </summary>
    Tasks,

    /// <summary>
    /// Employee cases feature
    /// </summary>
    EmployeeCases,

    /// <summary>
    /// Company cases feature
    /// </summary>
    CompanyCases,

    /// <summary>
    /// National cases feature
    /// </summary>
    NationalCases,

    /// <summary>
    /// Global cases feature
    /// </summary>
    GlobalCases,

    /// <summary>
    /// Reports feature
    /// </summary>
    Reports,

    #endregion

    #region Payrun

    /// <summary>
    /// Payrun results feature
    /// </summary>
    PayrunResults,

    /// <summary>
    /// Payrun jobs feature
    /// </summary>
    PayrunJobs,

    /// <summary>
    /// Payruns feature
    /// </summary>
    Payruns,

    #endregion

    #region Payroll

    /// <summary>
    /// Payrolls feature
    /// </summary>
    Payrolls,

    /// <summary>
    /// Payroll layers feature
    /// </summary>
    /// 
    PayrollLayers,

    /// <summary>
    /// Regulations feature
    /// </summary>
    /// 
    Regulations,

    /// <summary>
    /// Regulation feature
    /// </summary>
    Regulation,

    #endregion

    #region Administration

    /// <summary>
    /// Shared regulations feature
    /// </summary>
    SharedRegulations,

    /// <summary>
    /// Tenants feature
    /// </summary>
    /// 
    Tenants,

    /// <summary>
    /// Users feature
    /// </summary>
    /// 
    Users,

    /// <summary>
    /// Calendars feature
    /// </summary>
    /// 
    Calendars,

    /// <summary>
    /// Divisions feature
    /// </summary>
    /// 
    Divisions,

    /// <summary>
    /// Employees feature
    /// </summary>
    /// 
    Employees,

    /// <summary>
    /// Webhooks feature
    /// </summary>
    Webhooks,

    /// <summary>
    /// Logs feature
    /// </summary>
    Logs,

    #endregion

    #region Shared and System

    /// <summary>
    /// Forecasts feature
    /// </summary>
    Forecasts,

    /// <summary>
    /// User storage feature
    /// </summary>
    UserStorage

    #endregion

}
