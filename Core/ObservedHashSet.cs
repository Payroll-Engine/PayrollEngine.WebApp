using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PayrollEngine.WebApp;

public class ObservedHashSet<T> : HashSet<T>
{
    public AsyncEvent<T> Added { get; set; }
    public AsyncEvent<T> Removed { get; set; }

    public async Task AddAsync(T item)
    {
        if (item == null)
        {
            throw new ArgumentNullException(nameof(item));
        }
        Add(item);
        await OnAddedAsync(item);
    }

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

    public async Task RemoveAllAsync(Predicate<T> match)
    {
        var items = this.Where(x => match(x));
        foreach (var item in items)
        {
            await RemoveAsync(item);
        }
    }

    public async Task ClearAsync()
    {
        while (Count > 0)
        {
            await RemoveAsync(this.First());
        }
    }

    private async Task OnAddedAsync(T item) =>
        await (Added?.InvokeAsync(this, item) ?? Task.CompletedTask);

    private async Task OnRemovedAsync(T item) =>
        await (Removed?.InvokeAsync(this, item) ?? Task.CompletedTask);
}