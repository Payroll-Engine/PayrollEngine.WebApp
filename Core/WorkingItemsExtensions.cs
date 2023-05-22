
namespace PayrollEngine.WebApp;

public static class WorkingItemsExtensions
{
    // Tenant
    public static bool TenantView(this WorkingItems workingItems) =>
        (workingItems & WorkingItems.TenantView) == WorkingItems.TenantView;

    public static bool TenantChange(this WorkingItems workingItems) =>
        (workingItems & WorkingItems.TenantChange) == WorkingItems.TenantChange;

    public static bool TenantReset(this WorkingItems workingItems) =>
        (workingItems & WorkingItems.TenantReset) == WorkingItems.TenantReset;

    public static bool TenantAvailable(this WorkingItems workingItems) =>
        TenantView(workingItems) || TenantChange(workingItems) || TenantReset(workingItems);

    // Payroll
    public static bool PayrollView(this WorkingItems workingItems) =>
        (workingItems & WorkingItems.PayrollView) == WorkingItems.PayrollView;

    public static bool PayrollChange(this WorkingItems workingItems) =>
        (workingItems & WorkingItems.PayrollChange) == WorkingItems.PayrollChange;

    public static bool PayrollReset(this WorkingItems workingItems) =>
        (workingItems & WorkingItems.PayrollReset) == WorkingItems.PayrollReset;

    public static bool PayrollAvailable(this WorkingItems workingItems) =>
        PayrollView(workingItems) || PayrollChange(workingItems) || PayrollReset(workingItems);

    // Employee
    public static bool EmployeeView(this WorkingItems workingItems) =>
        (workingItems & WorkingItems.EmployeeView) == WorkingItems.EmployeeView;

    public static bool EmployeeChange(this WorkingItems workingItems) =>
        (workingItems & WorkingItems.EmployeeChange) == WorkingItems.EmployeeChange;

    public static bool EmployeeReset(this WorkingItems workingItems) =>
        (workingItems & WorkingItems.EmployeeReset) == WorkingItems.EmployeeReset;

    public static bool EmployeeAvailable(this WorkingItems workingItems) =>
        EmployeeView(workingItems) || EmployeeChange(workingItems) || EmployeeReset(workingItems);
}