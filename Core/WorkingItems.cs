using System;

namespace PayrollEngine.WebApp;

/// <summary>
/// Application working items
/// </summary>
[Flags]
public enum WorkingItems
{
    /// <summary>
    /// No working items
    /// </summary>
    None = 0x00000000,

    #region Tenant

    /// <summary>
    /// View tenants
    /// </summary>
    TenantView = 0x00000001,

    /// <summary>
    /// Change the tenant
    /// </summary>
    TenantChange = 0x00000002 | TenantView,

    /// <summary>
    /// Reset the tenant
    /// </summary>
    TenantReset = 0x00000004 | TenantChange,

    #endregion

    #region Payroll

    /// <summary>
    /// View payrolls
    /// </summary>
    PayrollView = 0x00000010,

    /// <summary>
    /// Change the payroll
    /// </summary>
    PayrollChange = 0x00000020 | PayrollView,

    /// <summary>
    /// Reset the payroll
    /// </summary>
    PayrollReset = 0x00000040 | PayrollChange,

    #endregion

    #region Employee

    /// <summary>
    /// View employees
    /// </summary>
    EmployeeView = 0x00000100,

    /// <summary>
    /// Change the employee
    /// </summary>
    EmployeeChange = 0x00000200 | EmployeeView,

    /// <summary>
    /// Reset the employee
    /// </summary>
    EmployeeReset = 0x00000400 | EmployeeChange,

    #endregion

    /// <summary>
    /// All working items
    /// </summary>
    All = TenantReset | PayrollReset | EmployeeReset

}