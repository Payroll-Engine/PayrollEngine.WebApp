using System.Collections.Generic;
using System.Linq;

namespace PayrollEngine.WebApp;

/// <summary>
/// Extension methods for <see cref="Dictionary{TKey,TValue}" />
/// </summary>
public static class DictionaryExtensions
{
    /// <summary>
    /// Checks if content of two dictionary is equal regardless of order
    /// </summary>
    /// <typeparam name="TKey">Type of dictionary key</typeparam>
    /// <typeparam name="TValue">Type of dictionary Value</typeparam>
    /// <param name="dictionary">Dictionary input</param>
    /// <param name="otherDictionary">Dictionary to compare</param>
    /// <returns>true if both dictionary have the same keys and values</returns>
    public static bool ContentEquals<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, Dictionary<TKey, TValue> otherDictionary)
    {
        return (otherDictionary ?? new Dictionary<TKey, TValue>())
            .OrderBy(kvp => kvp.Key)
            // ReSharper disable once UsageOfDefaultStructEquality
            .SequenceEqual((dictionary ?? new Dictionary<TKey, TValue>())
                .OrderBy(kvp => kvp.Key));
    }
}