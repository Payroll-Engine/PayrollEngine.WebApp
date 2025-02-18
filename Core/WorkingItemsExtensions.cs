namespace PayrollEngine.WebApp;

/// <summary>
/// Extension methods for <see cref="WorkingItems" />
/// </summary>
public static class WorkingItemsExtensions
{

    #region Tenant

    /// <summary>
    /// Test for tenant view working item
    /// </summary>
    /// <param name="workingItems">Working item to test</param>
    public static bool TenantView(this WorkingItems workingItems) =>
        (workingItems & WorkingItems.TenantView) == WorkingItems.TenantView;

    /// <summary>
    /// Test for tenant view working item
    /// </summary>
    public static bool TenantChange(this WorkingItems workingItems) =>
        (workingItems & WorkingItems.TenantChange) == WorkingItems.TenantChange;

    /// <summary>
    /// Test for tenant view working item
    /// </summary>
    public static bool TenantReset(this WorkingItems workingItems) =>
        (workingItems & WorkingItems.TenantReset) == WorkingItems.TenantReset;

    /// <summary>
    /// Test for tenant view working item
    /// </summary>
    public static bool TenantAvailable(this WorkingItems workingItems) =>
        TenantView(workingItems) || TenantChange(workingItems) || TenantReset(workingItems);

    #endregion

    #region Payroll

    /// <summary>
    /// Test for payroll view working item
    /// </summary>
    public static bool PayrollView(this WorkingItems workingItems) =>
        (workingItems & WorkingItems.PayrollView) == WorkingItems.PayrollView;

    /// <summary>
    /// Test for payroll change working item
    /// </summary>
    public static bool PayrollChange(this WorkingItems workingItems) =>
        (workingItems & WorkingItems.PayrollChange) == WorkingItems.PayrollChange;

    /// <summary>
    /// Test for payroll reset working item
    /// </summary>
    public static bool PayrollReset(this WorkingItems workingItems) =>
        (workingItems & WorkingItems.PayrollReset) == WorkingItems.PayrollReset;

    /// <summary>
    /// Test for payroll available working item
    /// </summary>
    public static bool PayrollAvailable(this WorkingItems workingItems) =>
        PayrollView(workingItems) || PayrollChange(workingItems) || PayrollReset(workingItems);

    #endregion

    #region Employee

    /// <summary>
    /// Test for employee view working item
    /// </summary>
    public static bool EmployeeView(this WorkingItems workingItems) =>
        (workingItems & WorkingItems.EmployeeView) == WorkingItems.EmployeeView;

    /// <summary>
    /// Test for employee change working item
    /// </summary>
    public static bool EmployeeChange(this WorkingItems workingItems) =>
        (workingItems & WorkingItems.EmployeeChange) == WorkingItems.EmployeeChange;

    /// <summary>
    /// Test for employee reset working item
    /// </summary>
    public static bool EmployeeReset(this WorkingItems workingItems) =>
        (workingItems & WorkingItems.EmployeeReset) == WorkingItems.EmployeeReset;

    /// <summary>
    /// Test for employee available working item
    /// </summary>
    public static bool EmployeeAvailable(this WorkingItems workingItems) =>
        EmployeeView(workingItems) || EmployeeChange(workingItems) || EmployeeReset(workingItems);

    #endregion

}