using System.Globalization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;

namespace PayrollEngine.WebApp;

/// <summary>Extension methods for <see cref="NavigationManager"/>
/// Do not name the class NavigationManagerExtensions</summary>
/// <remarks>source: https://chrissainty.com/working-with-query-strings-in-blazor/ </remarks>
public static class NavigationManagerExtensions
{
    /// <summary>
    /// Tries to parse and return a URL parameter based on string key
    /// </summary>
    /// <typeparam name="T">return Type</typeparam>
    /// <param name="navManager">NavigationManager Component</param>
    /// <param name="key">Parameter key</param>
    /// <param name="value">out value</param>
    /// <returns>url parameter</returns>
    public static bool TryParseQueryValue<T>(this NavigationManager navManager, string key, out T value)
    {
        var uri = navManager.ToAbsoluteUri(navManager.Uri);

        if (QueryHelpers.ParseQuery(uri.Query).TryGetValue(key, out var valueFromQueryString))
        {
            if (typeof(T) == typeof(int) &&
                int.TryParse(valueFromQueryString, NumberStyles.Any, CultureInfo.InvariantCulture, out var valueAsInt))
            {
                value = (T)(object)valueAsInt;
                return true;
            }

            if (typeof(T) == typeof(string))
            {
                value = (T)(object)valueFromQueryString.ToString();
                return true;
            }

            if (typeof(T) == typeof(decimal) &&
                decimal.TryParse(valueFromQueryString, NumberStyles.Any, CultureInfo.InvariantCulture, out var valueAsDecimal))
            {
                value = (T)(object)valueAsDecimal;
                return true;
            }
        }

        value = default;
        return false;
    }
}