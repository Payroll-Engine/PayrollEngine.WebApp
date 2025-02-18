using System;
using System.Linq;
using PayrollEngine.Client;
using PayrollEngine.Client.Service;
using Task = System.Threading.Tasks.Task;

namespace PayrollEngine.WebApp;

/// <summary>
/// Working items watcher
/// </summary>
/// <typeparam name="TService">Service to watch</typeparam>
/// <typeparam name="TServiceContext">Service context</typeparam>
/// <typeparam name="TItem">Item type</typeparam>
/// <typeparam name="TQuery">Item query type</typeparam>
public class WorkingItemsWatcher<TService, TServiceContext, TItem, TQuery>
    where TService : IReadService<TItem, TServiceContext, TQuery>
    where TServiceContext : IServiceContext
    where TItem : class, IModel, new()
    where TQuery : Query, new()
{
    private TService Service { get; }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="service">Watch service</param>
    public WorkingItemsWatcher(TService service)
    {
        if (service == null)
        {
            throw new ArgumentNullException(nameof(service));
        }
        Service = service;
    }

    /// <summary>
    /// Updated watch state
    /// </summary>
    /// <param name="items">Items</param>
    /// <param name="serviceContext">Service context</param>
    public async Task UpdateAsync(ItemCollection<TItem> items, TServiceContext serviceContext) =>
        await UpdateAsync(items, serviceContext, new()
        {
            Status = ObjectStatus.Active
        });

    /// <summary>
    /// Updated watch state
    /// </summary>
    /// <param name="items">Items</param>
    /// <param name="serviceContext">Service context</param>
    /// <param name="query">Service query</param>
    public async Task UpdateAsync(ItemCollection<TItem> items, TServiceContext serviceContext, TQuery query)
    {
        // retrieve all new items
        var newWorkingItems = await Service.QueryAsync<TItem>(serviceContext, query);

        // populate collection with working items, or expand if already exists
        var workingItemsToRemove = items.Where(t => !newWorkingItems.Select(n => n.Id).Contains(t.Id)).Select(x => x.Id).ToList();

        // remove working items not present in database anymore
        for (var i = items.Count - 1; i >= 0; i--)
        {
            if (workingItemsToRemove.Contains(items[i].Id))
            {
                items.RemoveAt(i);
            }
        }

        foreach (var newWorkingItem in newWorkingItems)
        {
            var existingWorkingItem = items.FirstOrDefault(t => t.Id == newWorkingItem.Id);
            if (existingWorkingItem != null)
            {
                // if there is an existing working item with the same identifier BUT they are not equal, update existing working item
                if (!newWorkingItem.Equals(existingWorkingItem))
                {
                    var index = items.IndexOf(existingWorkingItem);
                    if (index >= 0)
                    {
                        items[index] = newWorkingItem;
                    }
                }
            }
            else
            {
                // if there is no existing working item with same identifier, add new
                items.Add(newWorkingItem);
            }
        }
    }
}