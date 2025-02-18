using System;
using System.Threading;
using System.Threading.Tasks;
using Blazored.LocalStorage;

namespace PayrollEngine.WebApp;

/// <summary>
/// Extension methods for <see cref="ILocalStorageService" />
/// </summary>
public static class LocalStorageServiceExtensions
{
    /// <summary>
    /// Get local storage value as boolean
    /// </summary>
    /// <param name="storageService">Local storage service</param>
    /// <param name="key">Storage key</param>
    /// <param name="defaultValue">Default value</param>
    /// <param name="cancellationToken">Cancellation token</param>
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

    /// <summary>
    /// Set local storage boolean value
    /// </summary>
    /// <param name="storageService">Local storage service</param>
    /// <param name="key">Storage key</param>
    /// <param name="data">Storage data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    public static async Task SetItemAsBooleanAsync(this ILocalStorageService storageService,
        string key, bool data, CancellationToken cancellationToken = default) =>
        await storageService.SetItemAsStringAsync(key, data.ToString(), cancellationToken);
}
