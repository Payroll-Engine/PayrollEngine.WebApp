using System;
using System.Threading;
using System.Threading.Tasks;
using Blazored.LocalStorage;

namespace PayrollEngine.WebApp;

public static class LocalStorageServiceExtensions
{
    public static async Task<bool?> GetItemAsBooleanAsync(this ILocalStorageService storageService,
        string key, bool? defaultValue = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentException(nameof(key));
        }

        if (!await storageService.ContainKeyAsync(key, cancellationToken))
        {
            return defaultValue;
        }

        var value = await storageService.GetItemAsStringAsync(key, cancellationToken);
        if (!string.IsNullOrWhiteSpace(value) && bool.TryParse(value, out var boolValue))
        {
            return boolValue;
        }
        return defaultValue;
    }

    public static async Task SetItemAsBooleanAsync(this ILocalStorageService storageService,
        string key, bool data, CancellationToken cancellationToken = default) =>
        await storageService.SetItemAsStringAsync(key, data.ToString(), cancellationToken);
}
