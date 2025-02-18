using System.Threading.Tasks;
using System.Collections.Generic;

namespace PayrollEngine.WebApp;

/// <summary>
/// Forecast history service
/// </summary>
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
