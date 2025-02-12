using PayrollEngine.WebApp.ViewModel;
using Task = System.Threading.Tasks.Task;

namespace PayrollEngine.WebApp.Server.Components.Pages;

public interface IReportOperator
{
    Task ShowReportLogAsync(ReportSet report);
    Task BuildReportAsync(ReportSet report);
}