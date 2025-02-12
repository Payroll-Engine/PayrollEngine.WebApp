using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazored.LocalStorage;

namespace PayrollEngine.WebApp.Server.Components.Shared;

public class ForecastHistoryService(ILocalStorageService localStorage) : IForecastHistoryService
{
    private const int historySize = 20;
    private ILocalStorageService LocalStorage { get; } = localStorage ?? throw new ArgumentNullException(nameof(localStorage));

    public async Task<List<string>> GetHistoryAsync() =>
        await GetStorageHistoryAsync();

    public async Task AddHistoryAsync(string forecast)
    {
        if (string.IsNullOrWhiteSpace(forecast))
        {
            return;
        }

        var history = await GetStorageHistoryAsync();

        // move existing item to the top
        if (history.Contains(forecast))
        {
            history.Remove(forecast);
        }
        else if (history.Count >= historySize)
        {
            // max limit: remove oldest
            history.Remove(history.Last());
        }

        // add first item
        history.Insert(0, forecast);
        await SetStorageHistoryAsync(history);
    }

    private async Task<List<string>> GetStorageHistoryAsync()
    {
        var storage = await LocalStorage.GetItemAsStringAsync("Forecasts");
        return string.IsNullOrWhiteSpace(storage) ? [] : storage.Split('\t').ToList();
    }

    private async Task SetStorageHistoryAsync(IReadOnlyCollection<string> history)
    {
        if (!history.Any())
        {
            await LocalStorage.RemoveItemAsync("Forecasts");
        }
        else
        {
            await LocalStorage.SetItemAsStringAsync("Forecasts", string.Join('\t', history));
        }
    }
}