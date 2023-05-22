using System;

namespace PayrollEngine.WebApp;

[Flags]
public enum WorkingItems
{
    None = 0x00000000,

    // Tenant
    TenantView = 0x00000001,
    TenantChange = 0x00000002 | TenantView,
    TenantReset = 0x00000004 | TenantChange,

    // Payroll
    PayrollView = 0x00000010,
    PayrollChange = 0x00000020 | PayrollView,
    PayrollReset = 0x00000040 | PayrollChange,

    // Employee
    EmployeeView = 0x00000100,
    EmployeeChange = 0x00000200 | EmployeeView,
    EmployeeReset = 0x00000400 | EmployeeChange,

    All = TenantReset | PayrollReset | EmployeeReset
}