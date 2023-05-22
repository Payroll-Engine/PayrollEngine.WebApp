using System.Collections.Generic;
using System.Threading.Tasks;
using PayrollEngine.Client.Model;

namespace PayrollEngine.WebApp;

public interface ICaseValueProvider
{
    /// <summary>
    /// Retrieve all case values for given case field
    /// </summary>
    /// <param name="query">Query with additional parameters and filters</param>
    /// <returns></returns>
    Task<IEnumerable<CaseValueSetup>> GetCaseValuesAsync(CaseValueQuery query);
}