using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace PayrollEngine.WebApp;

/// <summary>
/// Observed hashset
/// </summary>
public class ObservedHashSet<T> : HashSet<T>
{
    public AsyncEvent<T> Added { get; set; }
    public AsyncEvent<T> Removed { get; set; }

    /// <summary>
    /// Add item
    /// </summary>
    /// <param name="item">Item to add</param>
    public async Task AddAsync(T item)
    {
        if (item == null)
        {
            throw new ArgumentNullException(nameof(item));
        }
        Add(item);
        await OnAddedAsync(item);
    }

    /// <summary>
    /// Add multiple items
    /// </summary>
    /// <param name="items">Items to add</param>
    public async Task AddRangeAsync(IEnumerable<T> items)
    {
        if (items == null)
        {
            throw new ArgumentNullException(nameof(items));
        }
        foreach (var item in items)
        {
            await AddAsync(item);
        }
    }

    /// <summary>
    /// Remove item
    /// </summary>
    /// <param name="item">Item to remove</param>
    public async Task RemoveAsync(T item)
    {
        if (item == null)
        {
            throw new ArgumentNullException(nameof(item));
        }
        var removed = Remove(item);
        if (removed)
        {
            await OnRemovedAsync(item);
        }
    }

    /// <summary>
    /// Remove all items
    /// </summary>
    /// <param name="match">Remove predicate</param>
    public async Task RemoveAllAsync(Predicate<T> match)
    {
        var items = this.Where(x => match(x));
        foreach (var item in items)
        {
            await RemoveAsync(item);
        }
    }

    /// <summary>
    /// Clear all items
    /// </summary>
    public async Task ClearAsync()
    {
        while (Count > 0)
        {
            await RemoveAsync(this.First());
        }
    }

    /// <summary>
    /// Added item handler
    /// </summary>
    /// <param name="item">Added item</param>
    private async Task OnAddedAsync(T item) =>
        await (Added?.InvokeAsync(this, item) ?? Task.CompletedTask);

    /// <summary>
    /// Removed item handler
    /// </summary>
    /// <param name="item">Removed item</param>
    private async Task OnRemovedAsync(T item) =>
        await (Removed?.InvokeAsync(this, item) ?? Task.CompletedTask);
}