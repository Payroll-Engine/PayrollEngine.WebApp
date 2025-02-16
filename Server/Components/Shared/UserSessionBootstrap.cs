using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using PayrollEngine.Client.QueryExpression;
using PayrollEngine.Client.Service;
using PayrollEngine.WebApp.Presentation;
using PayrollEngine.WebApp.ViewModel;
using Task = System.Threading.Tasks.Task;
using ClientModel = PayrollEngine.Client.Model;

namespace PayrollEngine.WebApp.Server.Components.Shared;

/// <summary>Bootstrap the user session</summary>
public class UserSessionBootstrap(IHostApplicationLifetime applicationLifetime,
    IConfiguration configuration)
{
    private IHostApplicationLifetime ApplicationLifetime { get; } = applicationLifetime;
    private IConfiguration Configuration { get; } = configuration;

    private UserSession LastUserSession { get; set; }

    public void Update(UserSession userSession)
    {
        if (!userSession.UserAvailable && LastUserSession != null)
        {
            userSession.ImportFrom(LastUserSession);
        }
    }

    /// <summary>
    /// Builds the user session <see cref="UserSession"/> based on the web app configuration <see cref="AppConfiguration"/>.
    /// Currently, the startup user and tenant is mandatory.
    /// </summary>
    /// <exception cref="Exception">Currently the startup user is required</exception>
    /// <exception cref="Exception">Currently the startup tenant is required</exception>
    /// <exception cref="Exception">Missing or invalid startup tenant {startupTenant}</exception>
    public async Task Apply(UserSession userSession,
        IUserService userService,
        ITaskService taskService,
        ITenantService tenantService,
        IEmployeeService employeeService,
        IPayrollService payrollService)
    {
        if (userSession == null)
        {
            throw new ArgumentNullException(nameof(userSession));
        }

        try
        {
            // mandatory application configuration
            var appConfiguration = Configuration.GetConfiguration<AppConfiguration>();
            if (appConfiguration == null)
            {
                throw new PayrollException("Missing application configuration.");
            }

            // default features
            if (appConfiguration.DefaultFeatures != null)
            {
                userSession.SetDefaultFeatures(appConfiguration.DefaultFeatures.Select(Enum.Parse<Feature>));
            }

            // optional startup configuration
            var startupConfiguration = Configuration.GetConfiguration<StartupConfiguration>();
            if (startupConfiguration != null && startupConfiguration.AutoLogin)
            {
                // startup objects
                User user = null;
                ClientModel.Payroll payroll = null;
                Employee employee = null;

                // startup working objects
                var tenant = await GetStartupTenant(tenantService, startupConfiguration.StartupTenant);
                if (tenant != null)
                {
                    // startup user
                    user = await GetStartupUser(userService, taskService, tenant, startupConfiguration.StartupUser);
                    if (user != null)
                    {
                        // startup payroll
                        payroll = await GetStartupPayroll(payrollService, tenant, user);
                        if (payroll != null)
                        {
                            // startup employee
                            employee = user.UserType == UserType.Employee ?
                                // user employee
                                await GetUserEmployee(employeeService, tenant, user) :
                                // regular employee or supervisor
                                await GetStartupEmployee(employeeService, tenant, user);
                        }
                    }
                }

                // default tenant (fallback)
                if (tenant == null)
                {
                    tenant = (await tenantService.QueryAsync<Tenant>(new())).FirstOrDefault();
                    if (tenant == null)
                    {
                        return;
                    }
                }

                // default user (fallback)
                if (user == null)
                {
                    user = (await userService.QueryAsync<User>(new(tenant.Id))).FirstOrDefault();
                    if (user == null)
                    {
                        return;
                    }
                    await SetupUserTasksAsync(taskService, tenant, user);
                }

                // user login
                await userSession.LoginAsync(tenant, user);

                // working payroll
                if (payroll != null)
                {
                    await userSession.ChangePayrollAsync(payroll);
                }

                // working employee
                if (employee != null)
                {
                    await userSession.ChangeEmployeeAsync(employee);
                }

                LastUserSession = userSession;
            }
        }
        catch (Exception exception)
        {
            Log.Critical(exception, exception.GetBaseMessage());
            // debug break point
            if (System.Diagnostics.Debugger.IsAttached)
            {
                System.Diagnostics.Debug.WriteLine($"!!! {exception.GetBaseMessage()} !!!");
            }
            ApplicationLifetime.StopApplication();
        }
    }

    private async Task<Tenant> GetStartupTenant(ITenantService tenantService, string startupTenant)
    {
        Tenant tenant = null;
        if (!string.IsNullOrWhiteSpace(startupTenant))
        {
            tenant = (await tenantService.QueryAsync<Tenant>(new())).FirstOrDefault(x =>
                string.Equals(x.Identifier, startupTenant));
        }
        return tenant;
    }

    private async Task<User> GetStartupUser(IUserService userService, ITaskService taskService, Tenant tenant, string startupUser)
    {
        if (string.IsNullOrWhiteSpace(startupUser))
        {
            return null;
        }
        var user = await userService.GetAsync<User>(new(tenant.Id), startupUser);
        if (user == null)
        {
            return null;
        }
        await SetupUserTasksAsync(taskService, tenant, user);
        return user;
    }

    private async Task SetupUserTasksAsync(ITaskService taskService, Tenant tenant, User user)
    {
        var tasks = await taskService.QueryAsync<Client.Model.Task>(new(tenant.Id), new()
        {
            Filter = new Equals(nameof(Client.Model.Task.ScheduledUserId), user.Id).
                And(new Equals(nameof(Client.Model.Task.Completed), null)).
                And(new LessFilter(nameof(Client.Model.Task.Scheduled), Date.Now))
        });
        user.OpenTaskCount = tasks.Count;
    }

    private async Task<ClientModel.Payroll> GetStartupPayroll(IPayrollService payrollService, Tenant tenant, User user)
    {
        ClientModel.Payroll payroll = null;
        var startupPayroll = user.Attributes.GetValue<string>(nameof(User.StartupPayroll));
        if (!string.IsNullOrWhiteSpace(startupPayroll))
        {
            payroll = await payrollService.GetAsync<ClientModel.Payroll>(new(tenant.Id),
                startupPayroll);
        }
        return payroll;
    }

    private async Task<Employee> GetUserEmployee(IEmployeeService employeeService, Tenant tenant, User user)
    {
        // user and employee must have the same identifier
        var employee = await employeeService.GetAsync<Employee>(new(tenant.Id), user.Identifier);
        return employee;
    }

    private async Task<Employee> GetStartupEmployee(IEmployeeService employeeService, Tenant tenant, User user)
    {
        Employee employee = null;
        var startupEmployee = user.Attributes.GetValue<string>(nameof(User.StartupEmployee));
        if (!string.IsNullOrWhiteSpace(startupEmployee))
        {
            employee = await employeeService.GetAsync<Employee>(new(tenant.Id), startupEmployee);
        }
        return employee;
    }
}

