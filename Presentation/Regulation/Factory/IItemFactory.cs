using System.Collections.Generic;
using System.Threading.Tasks;
using PayrollEngine.WebApp.ViewModel;

namespace PayrollEngine.WebApp.Presentation.Regulation.Factory;

public interface IItemFactory<TItem>
    where TItem : class, IRegulationItem
{
    /// <summary>
    /// Query current regulation items
    /// </summary>
    /// <returns>The item collection</returns>
    Task<List<TItem>> QueryPayrollItems();
}