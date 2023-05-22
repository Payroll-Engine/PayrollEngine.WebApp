using System.Collections.Generic;
using System.Threading.Tasks;
using MudBlazor;

namespace PayrollEngine.WebApp.Presentation;

/// <summary>Server service</summary>
/// <typeparam name="TItem">The backend item type</typeparam>
/// <typeparam name="TQuery">The query type</typeparam>
public interface IBackendService<TItem, in TQuery>
    where TItem : class, new()
    where TQuery : Query, new()
{
    /// <summary>Item limitation, default is unlimited</summary>
    int? MaximumItemCount { get; set; }

    /// <summary>Query server items</summary>
    /// <param name="query">The request query</param>
    /// <param name="parameters">The query parameters</param>
    /// <returns>Collection of items</returns>
    Task<GridData<TItem>> QueryAsync(TQuery query = null, IDictionary<string, object> parameters = null);

    /// <summary>Query server items</summary>
    /// <param name="state">The request state</param>
    /// <param name="resolver">The query resolver</param>
    /// <param name="parameters">The query parameters</param>
    /// <returns>Collection of items</returns>
    Task<GridData<TItem>> QueryAsync(GridState<TItem> state, IQueryResolver resolver = null, IDictionary<string, object> parameters = null);

    /// <summary>Get server item</summary>
    /// <param name="itemId">The item id</param>
    /// <returns>The item</returns>
    Task<TItem> GetAsync(int itemId);

    /// <summary>Create server item</summary>
    /// <param name="item">The item to create</param>
    /// <returns>The created item</returns>
    Task<TItem> CreateAsync(TItem item);

    /// <summary>Update server item</summary>
    /// <param name="item">The item to update</param>
    /// <returns>The updated item</returns>
    Task<TItem> UpdateAsync(TItem item);

    /// <summary>Delete server item</summary>
    /// <param name="itemId">The item id</param>
    /// <returns>True on deleted item</returns>
    Task<bool> DeleteAsync(int itemId);
}