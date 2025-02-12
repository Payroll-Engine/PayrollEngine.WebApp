using PayrollEngine.WebApp.ViewModel;
using Task = System.Threading.Tasks.Task;

namespace PayrollEngine.WebApp.Server.Components.Pages;

public interface IPayrunJobOperator
{
    Task ShowJobAsync(PayrunJob payrunJob);
    Task CopyForecastJobAsync(PayrunJob payrunJob);
}