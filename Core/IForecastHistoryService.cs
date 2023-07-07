using System.Collections.Generic;
using System.Threading.Tasks;

namespace PayrollEngine.WebApp;

public interface IForecastHistoryService
{
    /// <summary>
    /// Get the forecast history
    /// </summary>
    Task<List<string>> GetHistoryAsync();

    /// <summary>
    /// Add forecast history
    /// </summary>
    Task AddHistoryAsync(string forecast);
}
