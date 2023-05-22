using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using PayrollEngine.Client.Service;
using PayrollEngine.WebApp.Presentation;

namespace PayrollEngine.WebApp.Server.Shared;

public class UserSessionSettings
{
    public IHostApplicationLifetime ApplicationLifetime { get; set; }
    public IConfiguration Configuration { get; set; }
    public UserSession UserSession { get; set; }
    public IUserService UserService { get; set; }
    public ITenantService TenantService { get; set; }
    public IEmployeeService EmployeeService { get; set; }
    public IPayrollService PayrollService { get; set; }
}