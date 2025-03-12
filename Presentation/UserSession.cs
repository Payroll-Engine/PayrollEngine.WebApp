using System;
using System.Linq;
using System.Globalization;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using PayrollEngine.Client.Model;
using PayrollEngine.Client.Service;
using PayrollEngine.Client.QueryExpression;
using Task = System.Threading.Tasks.Task;
using Division = PayrollEngine.WebApp.ViewModel.Division;
using Employee = PayrollEngine.WebApp.ViewModel.Employee;
using Tenant = PayrollEngine.WebApp.ViewModel.Tenant;

namespace PayrollEngine.WebApp.Presentation;

/// <summary>
/// User session
/// </summary>
/// <param name="configuration">Configuration service</param>
/// <param name="cultureService">Culture service</param>
/// <param name="tenantService">Tenant service</param>
/// <param name="divisionService">Division service</param>
/// <param name="payrollService">Payroll service</param>
/// <param name="employeeService">Employee service</param>
/// <param name="userService">User service</param>
public class UserSession(IConfiguration configuration,
    ICultureService cultureService,
    ITenantService tenantService,
    IDivisionService divisionService,
    IPayrollService payrollService,
    IEmployeeService employeeService,
    IUserService userService)
    : IDisposable
{
    private IConfiguration Configuration { get; } = configuration;
    private ICultureService CultureService { get; } = cultureService;

    private readonly WorkingItemsWatcher<ITenantService, RootServiceContext, Tenant, Query> tenantWatcher = new(tenantService);
    private readonly WorkingItemsWatcher<IDivisionService, TenantServiceContext, Division, Query> divisionWatcher = new(divisionService);
    private readonly WorkingItemsWatcher<IPayrollService, TenantServiceContext, Payroll, Query> payrollWatcher = new(payrollService);
    private readonly WorkingItemsWatcher<IEmployeeService, TenantServiceContext, Employee, DivisionQuery> employeeWatcher = new(employeeService);

    [Inject]
    private ITaskService TaskService { get; set; }

    /// <summary>
    /// The value formatter 
    /// </summary>
    public IValueFormatter ValueFormatter { get; private set; }

    /// <summary>
    /// Default features if no features are present
    /// </summary>
    private List<Feature> DefaultFeatures { get; set; }

    /// <summary>
    /// Auto select children
    /// </summary>
    private static bool AutoSelectMode => true;

    #region Features

    /// <summary>
    /// Set the default user features
    /// </summary>
    /// <param name="defaultFeatures">The default features</param>
    public void SetDefaultFeatures(IEnumerable<Feature> defaultFeatures) =>
        DefaultFeatures = defaultFeatures.ToList();

    #endregion

    #region User

    // ReSharper disable once UnusedMember.Local

    private IUserService UserService { get; } = userService ?? throw new ArgumentNullException(nameof(userService));

    /// <summary>
    /// Session user
    /// </summary>
    public User User { get; private set; }

    /// <summary>
    /// Multitenant user
    /// </summary>
    public bool MultiTenantUser { get; private set; }

    /// <summary>
    /// Test user is available
    /// </summary>
    public bool UserAvailable => User != null;

    /// <summary>
    /// User changed event
    /// </summary>
    public AsyncEvent<User> UserChanged { get; set; }

    /// <summary>
    /// User login
    /// </summary>
    /// <param name="userTenant">Tenant oof the user</param>
    /// <param name="user">User</param>
    public async Task LoginAsync(Tenant userTenant, User user)
    {
        if (userTenant == null)
        {
            throw new ArgumentNullException(nameof(userTenant));
        }
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        // user features
        if (DefaultFeatures != null && !user.Features.Any() && !user.HasPassword)
        {
            user.Features = DefaultFeatures;
        }

        // user change
        User = user;
        await SetupUserTasks(user);

        // event
        await (UserChanged?.InvokeAsync(this, user) ?? Task.CompletedTask);

        // update tenant
        await ChangeTenantAsync(userTenant, user);

        // update state
        UpdateUserState();
    }

    /// <summary>
    /// Update the current user state, including culture and value formatter
    /// </summary>
    public void UpdateUserState()
    {
        var cultureName = GetUserCulture()  ?? CultureInfo.CurrentCulture.Name;
        var culture = CultureService.GetCulture(cultureName).CultureInfo;
        ValueFormatter = new ValueFormatter(culture);
    }

    /// <summary>
    /// Get the user culture
    /// </summary>
    public string GetUserCulture()
    {
        if (Tenant == null || User == null)
        {
            return null;
        }

        // [culture by priority]: user > tenant > system
        var culture =
            // priority 1: user culture
            User.Culture ??
            // priority 2: tenant culture
            Tenant.Culture ??
            // priority 3: system culture
            CultureInfo.CurrentCulture.Name;
        return culture;
    }

    /// <summary>
    /// Setup user tasks
    /// </summary>
    /// <param name="user">User to set up the tasks</param>
    private async Task SetupUserTasks(User user)
    {
        if (TaskService == null)
        {
            return;
        }

        var tasks = await TaskService.QueryAsync<Client.Model.Task>(new(Tenant.Id), new()
        {
            Filter = new Equals(nameof(Client.Model.Task.ScheduledUserId), user.Id).
                And(new Equals(nameof(Client.Model.Task.Completed), null)).
                And(new LessFilter(nameof(Client.Model.Task.Scheduled), Date.Now))
        });
        user.OpenTaskCount = tasks.Count;
    }

    /// <summary>
    /// Logout user
    /// </summary>
    public async Task LogoutAsync()
    {
        if (User == null)
        {
            return;
        }
        User = null;
        await ChangeTenantAsync(null);
    }

    /// <summary>
    /// Test for any user feature
    /// </summary>
    public bool AnyUserFeature() =>
        User != null && User.HasAnyFeature();

    /// <summary>
    /// Test for user feature
    /// </summary>
    /// <param name="feature">Test feature</param>
    public bool UserFeature(Feature feature) =>
        User != null && User.HasFeature(feature);

    #endregion

    #region Tenant

    private Tenant tenant;
    private ITenantService TenantService { get; } = tenantService ?? throw new ArgumentNullException(nameof(tenantService));

    /// <summary>
    /// Session tenant
    /// </summary>
    public Tenant Tenant => tenant;

    /// <summary>
    /// Available tenants
    /// </summary>
    public ItemCollection<Tenant> Tenants { get; } = new();

    /// <summary>
    /// Tenant changed event
    /// </summary>
    public AsyncEvent<Tenant> TenantChanged { get; set; }

    /// <summary>
    /// Set tenant
    /// </summary>
    /// <param name="newTenant">Tenant to set</param>
    private void SetTenant(Tenant newTenant)
    {
        tenant = newTenant;
    }

    /// <summary>
    /// Change the working tenant
    /// </summary>
    /// <param name="newTenant">The new tenant</param>
    /// <param name="user">The user</param>
    /// <remarks>Debug only: on empty user, the first available user wil be used</remarks>
    // ReSharper disable once UnusedParameter.Global
    public async Task ChangeTenantAsync(Tenant newTenant, User user = null)
    {
        // no changes
        if (tenant == newTenant)
        {
            return;
        }

        // logout
        if (newTenant == null)
        {
            ResetSession();
            return;
        }

        // tenants
        await SetupTenantsAsync();

        // available tenant
        var target = Tenants.FirstOrDefault(x => string.Equals(x.Identifier, newTenant.Identifier));
        if (target == null)
        {
            Log.Error($"Invalid tenant {newTenant.Identifier}");
            return;
        }
        // verify tenant
        var existingTenant = await TenantService.GetAsync<Tenant>(new(), target.Id);
        if (existingTenant == null)
        {
            // removed tenant
            Tenants.Remove(tenant);
            Log.Warning($"Removed tenant {tenant.Identifier}");
            // re-read updated tenants
            await SetupTenantsAsync();
            existingTenant = Tenants.FirstOrDefault(x => string.Equals(x.Identifier, newTenant.Identifier));
        }
        if (existingTenant == null)
        {
            return;
        }

        // change tenant
        SetTenant(target);

        // user
#if DEBUG
        user ??= (await UserService.QueryAsync<User>(new(tenant.Id))).FirstOrDefault();
        if (user == null)
        {
            throw new PayrollException($"Tenant {tenant.Identifier} without user.");
        }
        // user switch
        if (user != User)
        {
            // user change
            User = user;
            // features
            if (DefaultFeatures != null && !user.Features.Any() || !user.HasPassword)
            {
                User.Features = DefaultFeatures;
            }

            // event
            await (UserChanged?.InvokeAsync(this, user) ?? Task.CompletedTask);
        }
#endif

        // multi tenant user
        var appConfiguration = Configuration.GetConfiguration<AppConfiguration>();
        MultiTenantUser = appConfiguration.AllowTenantSwitch && await IsMultiTenantUserAsync(user, Tenants);

        // divisions
        await SetupDivisionsAsync();

        // payrolls
        SetPayroll(null);
        await SetupPayrollsAsync();

        // employees
        SetEmployee(null);
        Employees.Clear();

        // event
        await (TenantChanged?.InvokeAsync(this, tenant) ?? Task.CompletedTask);

        // auto select payroll
        if (AutoSelectMode)
        {
            if (Payrolls.Count == 1)
            {
                await ChangePayrollAsync(Payrolls.First());
            }
            else if (User != null && !string.IsNullOrWhiteSpace(User.StartupPayroll))
            {
                var startupPayroll = Payrolls.FirstOrDefault(x => string.Equals(x.Name, User.StartupPayroll));
                if (startupPayroll != null)
                {
                    await ChangePayrollAsync(startupPayroll);
                }
            }
        }

        // log
        Log.Trace($"Tenant changed to {tenant.Identifier}");
    }

    /// <summary>
    /// Setup tenant
    /// </summary>
    private async Task SetupTenantsAsync()
    {
        try
        {
            // retrieve and add all tenants
            await tenantWatcher.UpdateAsync(Tenants, new());
        }
        catch (Exception exception)
        {
            Log.Error(exception, exception.GetBaseMessage());
        }
    }

    /// <summary>
    /// Test for multi-tenant user
    /// </summary>
    private async System.Threading.Tasks.Task<bool> IsMultiTenantUserAsync(User user, IEnumerable<Tenant> tenants)
    {
        var count = 0;
        foreach (var curTenant in tenants)
        {
            var tenantUser = await UserService.GetAsync<ViewModel.User>(new(curTenant.Id), user.Identifier);
            if (tenantUser == null)
            {
                continue;
            }
            count++;
            if (count > 1)
            {
                break;
            }
        }
        return count > 1;
    }

    #endregion

    #region Division

    private IDivisionService DivisionService { get; } = divisionService ?? throw new ArgumentNullException(nameof(divisionService));

    /// <summary>
    /// Session division
    /// </summary>
    public ItemCollection<Division> Divisions { get; } = new();

    /// <summary>
    /// Setup division
    /// </summary>
    private async Task SetupDivisionsAsync()
    {
        try
        {
            // retrieve and add all division
            if (Tenant != null)
            {
                await divisionWatcher.UpdateAsync(Divisions, new(Tenant));
            }
        }
        catch (Exception exception)
        {
            Log.Error(exception, exception.GetBaseMessage());
        }
    }

    #endregion

    #region Payroll

    private Payroll payroll;
    public ItemCollection<Payroll> Payrolls { get; } = new();
    public Payroll Payroll => payroll;
    public AsyncEvent<Payroll> PayrollChanged { get; set; }

    private void SetPayroll(Payroll newPayroll)
    {
        payroll = newPayroll;
    }

    public async Task ChangePayrollAsync(Payroll newPayroll)
    {
        // no changes
        if (payroll == newPayroll)
        {
            return;
        }

        if (newPayroll != null)
        {
            if (Tenant == null)
            {
                throw new InvalidOperationException("Working tenant required for working payroll");
            }
            var target = Payrolls.FirstOrDefault(x => string.Equals(x.Name, newPayroll.Name));
            if (target == null)
            {
                Log.Error($"Invalid payroll {newPayroll.Name}");
                return;
            }
            newPayroll = target;
        }

        // change payroll
        SetPayroll(newPayroll);

        // employees
        SetEmployee(null);
        await SetupEmployeesAsync();

        // event
        await (PayrollChanged?.InvokeAsync(this, payroll) ?? Task.CompletedTask);

        // auto select employee
        if (AutoSelectMode)
        {
            Employee startupEmployee = null;
            // user employee
            if (User.UserType == UserType.Employee)
            {
                // user and employee must have the same identifier
                startupEmployee = Employees.FirstOrDefault(x => string.Equals(x.Identifier, User.Identifier));
                if (startupEmployee == null)
                {
                    throw new PayrollException($"Missing user employee: {User.Identifier}.");
                }
            }
            else
            {
                // regular single employee
                if (Employees.Count == 1)
                {
                    startupEmployee = Employees.First();
                }
                // regular multi employee: user startup setting
                else if (!string.IsNullOrWhiteSpace(User.StartupEmployee))
                {
                    startupEmployee = Employees.FirstOrDefault(x => string.Equals(x.Identifier, User.StartupEmployee));
                }
            }

            // employee change
            if (startupEmployee != null)
            {
                await ChangeEmployeeAsync(startupEmployee);
            }
        }
    }

    /// <summary>
    /// Setup payrolls
    /// </summary>
    private async Task SetupPayrollsAsync()
    {
        try
        {
            // tenant is required
            if (Tenant != null)
            {
                await payrollWatcher.UpdateAsync(Payrolls, new(Tenant));

                // setup derived payroll divisions
                foreach (var test in Payrolls)
                {
                    var division = Divisions.FirstOrDefault(x => x.Id == test.DivisionId);
                    if (division == null)
                    {
                        throw new PayrollException($"Missing division for derived payroll {test}.");
                    }
                    test.DivisionName = division.Name;
                }
            }
            else
            {
                Payrolls.Clear();
            }
        }
        catch (Exception exception)
        {
            Log.Error(exception, exception.GetBaseMessage());
        }
    }

    private void ResetPayrolls()
    {
        SetPayroll(null);
        Payrolls.Clear();
        ResetEmployees();
    }

    #endregion

    #region Employee

    private Employee employee;

    /// <summary>
    /// Session employee
    /// </summary>
    public Employee Employee => employee;

    /// <summary>
    /// Available employees
    /// </summary>
    public ItemCollection<Employee> Employees { get; } = new();

    /// <summary>
    /// Employee changed event
    /// </summary>
    public AsyncEvent<Employee> EmployeeChanged { get; set; }

    /// <summary>
    /// Set employee
    /// </summary>
    /// <param name="newEmployee">Employee to set</param>
    private void SetEmployee(Employee newEmployee)
    {
        employee = newEmployee;
    }

    /// <summary>
    /// Change employee
    /// </summary>
    /// <param name="newEmployee">Employee to set</param>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task ChangeEmployeeAsync(Employee newEmployee)
    {
        // no changes
        if (employee == newEmployee)
        {
            return;
        }

        if (newEmployee != null)
        {
            if (Tenant == null)
            {
                throw new InvalidOperationException("Working tenant required for working employee");
            }
            if (Payroll == null)
            {
                throw new InvalidOperationException("Working payroll required for working employee");
            }
            var target = Employees.FirstOrDefault(x => string.Equals(x.Identifier, newEmployee.Identifier));
            if (target == null)
            {
                Log.Error($"Invalid payroll {newEmployee.Identifier}");
                return;
            }
            newEmployee = target;
        }

        // change employee
        SetEmployee(newEmployee);

        // event
        await (EmployeeChanged?.InvokeAsync(this, employee) ?? Task.CompletedTask);
    }

    /// <summary>
    /// Setup employees
    /// </summary>
    private async Task SetupEmployeesAsync()
    {
        try
        {
            // tenant and derived payroll is required
            if (Tenant != null && Payroll != null)
            {
                var division = await DivisionService.GetAsync<Division>(new(Tenant), Payroll.DivisionId);
                if (division == null)
                {
                    throw new PayrollException($"Missing division in derived payroll {Payroll}.");
                }

                // retrieve all employees
                var query = new DivisionQuery { Status = ObjectStatus.Active, DivisionId = division.Id };
                await employeeWatcher.UpdateAsync(Employees, new(Tenant), query);
            }
            else
            {
                Employees.Clear();
            }
        }
        catch (Exception exception)
        {
            Log.Error(exception, exception.GetBaseMessage());
        }
    }

    /// <summary>
    /// Reset employees
    /// </summary>
    private void ResetEmployees()
    {
        SetEmployee(null);
        Employees.Clear();
    }

    #endregion

    #region Global

    /// <summary>
    /// User notifications
    /// </summary>
    public IUserNotificationService UserNotification { get; set; }

    /// <summary>
    /// Import user session
    /// </summary>
    /// <param name="source">Import source</param>
    public void ImportFrom(UserSession source)
    {
        ValueFormatter = source.ValueFormatter;

        User = source.User;
        MultiTenantUser = source.MultiTenantUser;

        tenant = source.tenant;
        Tenants.Clear();
        Tenants.AddRange(source.Tenants);
        Tenants.Clear();
        Tenants.AddRange(source.Tenants);

        Divisions.Clear();
        Divisions.AddRange(source.Divisions);

        payroll = source.payroll;
        Payrolls.Clear();
        Payrolls.AddRange(source.Payrolls);

        employee = source.employee;
        Employees.Clear();
        Employees.AddRange(source.Employees);
    }

    /// <summary>
    /// Reset session
    /// </summary>
    private void ResetSession()
    {
        SetTenant(null);
        Tenants.Clear();
        ResetPayrolls();
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Tenants?.Dispose();
        Divisions?.Dispose();
        Payrolls?.Dispose();
        Employees?.Dispose();
    }

    #endregion

}