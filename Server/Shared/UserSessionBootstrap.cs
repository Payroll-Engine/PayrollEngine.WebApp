using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using PayrollEngine.Client.QueryExpression;
using PayrollEngine.Client.Service;
using PayrollEngine.WebApp.Presentation;
using ClientModel = PayrollEngine.Client.Model;
using PayrollEngine.WebApp.ViewModel;
using Task = System.Threading.Tasks.Task;

namespace PayrollEngine.WebApp.Server.Shared;

/// <summary>Bootstrap the user session</summary>
public class UserSessionBootstrap
{
    public IHostApplicationLifetime ApplicationLifetime { get; }
    public IConfiguration Configuration { get; }
    public UserSession UserSession { get; }
    public IUserService UserService { get; }
    public ITaskService TaskService { get; }
    public ITenantService TenantService { get; }
    public IEmployeeService EmployeeService { get; }
    public IPayrollService PayrollService { get; }

    public UserSessionBootstrap(IHostApplicationLifetime applicationLifetime,
        IConfiguration configuration,
        UserSession userSession,
        IUserService userService,
        ITaskService taskService,
        ITenantService tenantService,
        IEmployeeService employeeService,
        IPayrollService payrollService)
    {
        ApplicationLifetime = applicationLifetime;
        Configuration = configuration;
        UserSession = userSession;
        UserService = userService;
        TaskService = taskService;
        TenantService = tenantService;
        EmployeeService = employeeService;
        PayrollService = payrollService;
    }

    /// <summary>
    /// Builds the user session <see cref="UserSession"/> based on the web app configuration <see cref="AppConfiguration"/>.
    /// Currently the startup user and tenant is mandatory.
    /// </summary>
    /// <exception cref="Exception">Currently the startup user is required</exception>
    /// <exception cref="Exception">Currently the startup tenant is required</exception>
    /// <exception cref="Exception">Missing or invalid startup tenant {startupTenant}</exception>
    public async Task Start()
    {
        // already started
        if (UserSession.UserAvailable)
        {
            return;
        }

        try
        {
            // mandatory application configuration
            var appConfiguration = Configuration.GetConfiguration<AppConfiguration>();
            if (appConfiguration == null)
            {
                throw new PayrollException("Missing application configuration");
            }
            // optional startup configuration
            var startupConfiguration = Configuration.GetConfiguration<StartupConfiguration>();


            // features
            if (appConfiguration.DefaultFeatures != null)
            {
                UserSession.DefaultFeatures = appConfiguration.DefaultFeatures.Select(Enum.Parse<Feature>).ToList();
            }

            if (startupConfiguration.AutoLogin)
            {
                // startup objects
                User user = null;
                ClientModel.Payroll payroll = null;
                Employee employee = null;

                // startup working objects
                var tenant = await GetStartupTenant(startupConfiguration.StartupTenant);
                if (tenant != null)
                {
                    // startup user
                    user = await GetStartupUser(tenant, startupConfiguration.StartupUser);
                    if (user != null)
                    {
                        // startup payroll
                        payroll = await GetStartupPayroll(tenant, user);
                        if (payroll != null)
                        {
                            // startup employee
                            employee = user.UserType == UserType.Employee ?
                                // user employee
                                await GetUserEmployee(tenant, user) :
                                // regular employee or supervisor
                                await GetStartupEmployee(tenant, user);
                        }
                    }
                }

                // default tenant (fallback)
                if (tenant == null)
                {
                    tenant = (await TenantService.QueryAsync<Tenant>(new())).FirstOrDefault();
                    if (tenant == null)
                    {
                        throw OnError("Missing tenant");
                    }
                }

                // default user (fallback)
                if (user == null)
                {
                    user = (await UserService.QueryAsync<User>(new(tenant.Id))).FirstOrDefault();
                    if (user == null)
                    {
                        throw OnError($"Missing user in tenant {tenant.Identifier}");
                    }
                    await SetupUserTasksAsync(tenant, user);
                }

                // user culture
                var culture = CultureTool.GetCulture(user.Culture);
                // set new value formatter
                UserSession.ValueFormatter = new ValueFormatter(culture);

                // user login
                await UserSession.LoginAsync(tenant, user);

                // working payroll
                if (payroll != null)
                {
                    await UserSession.ChangePayrollAsync(payroll);
                }

                // working employee
                if (employee != null)
                {
                    await UserSession.ChangeEmployeeAsync(employee);
                }
            }
        }
        catch (Exception exception)
        {
            Log.Critical(exception, exception.GetBaseMessage());
            OnError(exception);
            ApplicationLifetime.StopApplication();
        }
    }

    private async Task<Tenant> GetStartupTenant(string startupTenant)
    {
        Tenant tenant = null;
        if (!string.IsNullOrWhiteSpace(startupTenant))
        {
            tenant = (await TenantService.QueryAsync<Tenant>(new())).FirstOrDefault(x =>
                string.Equals(x.Identifier, startupTenant));
            if (tenant == null)
            {
                throw OnError($"Unknown startup tenant: {startupTenant}");
            }
        }
        return tenant;
    }

    private async Task<User> GetStartupUser(Tenant tenant, string startupUser)
    {
        if (string.IsNullOrWhiteSpace(startupUser))
        {
            return null;
        }
        var user = await UserService.GetAsync<User>(new(tenant.Id), startupUser);
        if (user == null)
        {
            throw OnError($"Unknown startup user: {startupUser}");
        }
        await SetupUserTasksAsync(tenant, user);
        return user;
    }

    private async Task SetupUserTasksAsync(Tenant tenant, User user)
    {
        var tasks = await TaskService.QueryAsync<Client.Model.Task>(new(tenant.Id), new()
        {
            Filter = new Equals(nameof(Client.Model.Task.ScheduledUserId), user.Id).
                And(new Equals(nameof(Client.Model.Task.Completed), null)).
                And(new LessFilter(nameof(Client.Model.Task.Scheduled), Date.Now))
        });
        user.OpenTaskCount = tasks.Count;
    }

    private async Task<ClientModel.Payroll> GetStartupPayroll(Tenant tenant, User user)
    {
        ClientModel.Payroll payroll = null;
        var startupPayroll = user.Attributes.GetValue<string>(nameof(User.StartupPayroll));
        if (!string.IsNullOrWhiteSpace(startupPayroll))
        {
            payroll = await PayrollService.GetAsync<ClientModel.Payroll>(new(tenant.Id),
                startupPayroll);
            if (payroll == null)
            {
                throw OnError($"Unknown startup payroll: {startupPayroll}");
            }
        }
        return payroll;
    }

    private async Task<Employee> GetUserEmployee(Tenant tenant, User user)
    {
        // user and employee must have the same identifier
        var employee = await EmployeeService.GetAsync<Employee>(new(tenant.Id), user.Identifier);
        if (employee == null)
        {
            throw OnError($"Missing user employee: {user.Identifier}");
        }
        return employee;
    }

    private async Task<Employee> GetStartupEmployee(Tenant tenant, User user)
    {
        Employee employee = null;
        var startupEmployee = user.Attributes.GetValue<string>(nameof(User.StartupEmployee));
        if (!string.IsNullOrWhiteSpace(startupEmployee))
        {
            employee = await EmployeeService.GetAsync<Employee>(new(tenant.Id), startupEmployee);
            if (employee == null)
            {
                throw OnError($"Unknown startup employee: {startupEmployee}");
            }
        }
        return employee;
    }

    private static Exception OnError(string message) =>
        OnError(new PayrollException(message));

    private static Exception OnError(Exception exception)
    {
#if DEBUG
        // debug break point
        if (Debugger.IsAttached)
        {
            Debug.WriteLine($"!!! {exception.GetBaseMessage()} !!!");
        }
#endif
        return exception;
    }
}