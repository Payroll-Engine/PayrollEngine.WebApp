﻿using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
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
    public ITenantService TenantService { get; }
    public IEmployeeService EmployeeService { get; }
    public IPayrollService PayrollService { get; }

    public UserSessionBootstrap(IHostApplicationLifetime applicationLifetime,
        IConfiguration configuration,
        UserSession userSession,
        IUserService userService,
        ITenantService tenantService,
        IEmployeeService employeeService,
        IPayrollService payrollService)
    {
        ApplicationLifetime = applicationLifetime;
        Configuration = configuration;
        UserSession = userSession;
        UserService = userService;
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
            UserSession.DefaultFeatures = appConfiguration.DefaultFeatures;

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
                            employee = await GetStartupEmployee(tenant, user);
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
                }

                // user culture
                var culture = CultureTool.GetCulture(user.Language.LanguageCode());
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
        User user = null;
        if (!string.IsNullOrWhiteSpace(startupUser))
        {
            user = await UserService.GetAsync<User>(new(tenant.Id), startupUser);
            if (user == null)
            {
                throw OnError($"Unknown startup user: {startupUser}");
            }
        }
        return user;
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