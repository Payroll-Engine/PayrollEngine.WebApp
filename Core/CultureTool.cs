using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace PayrollEngine.WebApp;

/// <summary>
/// Represent the ISO 4217 currency
/// see https://stackoverflow.com/a/12373900
/// </summary>
public static class CultureTool
{
    private static readonly IDictionary<string, CultureInfo> CultureInfos =
        new Dictionary<string, CultureInfo>();

    static CultureTool()
    {
        var cultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
        foreach (var culture in cultures)
        {
            if (!culture.IsNeutralCulture && !string.IsNullOrWhiteSpace(culture.Name))
            {
                CultureInfos.Add(culture.Name, culture);
            }
        }
    }

    public static List<Tuple<string, string, string>> GetCultureInfos() =>
        CultureInfo.GetCultures(CultureTypes.SpecificCultures).
            OrderBy(x => x.DisplayName).
            Select(x => new Tuple<string, string, string>(x.Name, x.DisplayName, x.EnglishName)).
            ToList();

    public static IEnumerable<string> GetCultureNames() =>
        CultureInfos.Values.Select(x => x.Name);

    public static string GetDisplayName(string cultureName) =>
        string.IsNullOrWhiteSpace(cultureName) ? null : GetCulture(cultureName)?.DisplayName;

    public static string GetIsoCurrencySymbol(string cultureName)
    {
        if (string.IsNullOrWhiteSpace(cultureName))
        {
            throw new ArgumentException(nameof(cultureName));
        }
        var cultureInfo = GetCulture(cultureName);
        return cultureInfo == null ? null : new RegionInfo(cultureName).ISOCurrencySymbol;
    }

    public static CultureInfo GetCulture(string cultureName)
    {
        // matching culture
        if (CultureInfos.TryGetValue(cultureName, out var culture))
        {
            return culture;
        }

        // matching language
        foreach (var key in CultureInfos.Keys)
        {
            if (key.StartsWith(cultureName))
            {
                return CultureInfos[key];
            }
        }

        return CultureInfo.CurrentCulture;
    }
}