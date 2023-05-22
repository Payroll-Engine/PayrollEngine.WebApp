using System.Collections.Generic;
using System.Threading.Tasks;
using PayrollEngine.WebApp.ViewModel;

namespace PayrollEngine.WebApp.Presentation.Regulation.Factory;

public interface IItemFactory<TItem>
    where TItem : class, IRegulationItem
{
    /// <summary>
    /// Query regulation items
    /// </summary>
    /// <param name="regulation">The regulation</param>
    /// <returns>The item collection</returns>
    Task<List<TItem>> QueryItems(Client.Model.Regulation regulation);

    /// <summary>
    /// Query current regulation items
    /// </summary>
    /// <returns>The item collection</returns>
    Task<List<TItem>> QueryPayrollItems();

    /// <summary>
    /// Load current regulation items
    /// </summary>
    /// <returns>The item collection</returns>
    Task<List<TItem>> LoadPayrollItems();

    /// <summary>
    /// Save regulation item
    /// </summary>
    /// <param name="list">The item collection</param>
    /// <param name="item">The item to save</param>
    /// <returns>True if item was saved successfully</returns>
    Task<bool> SaveItem(ICollection<TItem> list, TItem item);

    /// <summary>
    /// Delete item
    /// </summary>
    /// <param name="list">The item collection</param>
    /// <param name="item">The item to save</param>
    /// <returns>The deleted item</returns>
    Task<TItem> DeleteItem(ICollection<TItem> list, TItem item);
}