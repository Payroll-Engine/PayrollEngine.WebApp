using System.Threading.Tasks;
using System.Collections.Generic;
using PayrollEngine.Client.Model;

namespace PayrollEngine.WebApp;

/// <summary>
/// Case value provider
/// </summary>
public interface ICaseValueProvider
{
    /// <summary>
    /// Retrieve all case values for given case field
    /// </summary>
    /// <param name="query">Query with additional parameters and filters</param>
    Task<IEnumerable<CaseValueSetup>> GetCaseValuesAsync(CaseValueQuery query);
}