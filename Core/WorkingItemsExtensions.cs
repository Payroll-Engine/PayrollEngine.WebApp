namespace PayrollEngine.WebApp;

/// <summary>
/// Extension methods for <see cref="WorkingItems" />
/// </summary>
public static class WorkingItemsExtensions
{

    #region Tenant

    /// <param name="workingItems">Working item to test</param>
    extension(WorkingItems workingItems)
    {
        /// <summary>
        /// Test for tenant view working item
        /// </summary>
        public bool TenantView() =>
            (workingItems & WorkingItems.TenantView) == WorkingItems.TenantView;

        /// <summary>
        /// Test for tenant view working item
        /// </summary>
        public bool TenantChange() =>
            (workingItems & WorkingItems.TenantChange) == WorkingItems.TenantChange;

        /// <summary>
        /// Test for tenant view working item
        /// </summary>
        public bool TenantReset() =>
            (workingItems & WorkingItems.TenantReset) == WorkingItems.TenantReset;

        /// <summary>
        /// Test for tenant view working item
        /// </summary>
        public bool TenantAvailable() => workingItems.TenantView() || workingItems.TenantChange() || workingItems.TenantReset();
    }

    #endregion

    #region Payroll

    extension(WorkingItems workingItems)
    {
        /// <summary>
        /// Test for payroll view working item
        /// </summary>
        public bool PayrollView() =>
            (workingItems & WorkingItems.PayrollView) == WorkingItems.PayrollView;

        /// <summary>
        /// Test for payroll change working item
        /// </summary>
        public bool PayrollChange() =>
            (workingItems & WorkingItems.PayrollChange) == WorkingItems.PayrollChange;

        /// <summary>
        /// Test for payroll reset working item
        /// </summary>
        public bool PayrollReset() =>
            (workingItems & WorkingItems.PayrollReset) == WorkingItems.PayrollReset;

        /// <summary>
        /// Test for payroll available working item
        /// </summary>
        public bool PayrollAvailable() => workingItems.PayrollView() || workingItems.PayrollChange() || workingItems.PayrollReset();
    }

    #endregion

    #region Employee

    extension(WorkingItems workingItems)
    {
        /// <summary>
        /// Test for employee view working item
        /// </summary>
        public bool EmployeeView() =>
            (workingItems & WorkingItems.EmployeeView) == WorkingItems.EmployeeView;

        /// <summary>
        /// Test for employee change working item
        /// </summary>
        public bool EmployeeChange() =>
            (workingItems & WorkingItems.EmployeeChange) == WorkingItems.EmployeeChange;

        /// <summary>
        /// Test for employee reset working item
        /// </summary>
        public bool EmployeeReset() =>
            (workingItems & WorkingItems.EmployeeReset) == WorkingItems.EmployeeReset;

        /// <summary>
        /// Test for employee available working item
        /// </summary>
        public bool EmployeeAvailable() => workingItems.EmployeeView() || workingItems.EmployeeChange() || workingItems.EmployeeReset();
    }

    #endregion

}