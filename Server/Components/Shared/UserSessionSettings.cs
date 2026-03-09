using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using PayrollEngine.Client.Service;
using PayrollEngine.WebApp.Presentation;

namespace PayrollEngine.WebApp.Server.Components.Shared;

/// <summary>
/// Settings for user session initialization
/// </summary>
public class UserSessionSettings
{
    /// <summary>The application lifetime</summary>
    public IHostApplicationLifetime ApplicationLifetime { get; set; }
    /// <summary>The application configuration</summary>
    public IConfiguration Configuration { get; set; }
    /// <summary>The user session</summary>
    public UserSession UserSession { get; set; }
    /// <summary>The user service</summary>
    public IUserService UserService { get; set; }
    /// <summary>The tenant service</summary>
    public ITenantService TenantService { get; set; }
    /// <summary>The employee service</summary>
    public IEmployeeService EmployeeService { get; set; }
    /// <summary>The payroll service</summary>
    public IPayrollService PayrollService { get; set; }
}