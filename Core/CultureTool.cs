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

    public static List<string> GetSpecificCultureNames()
    {
        var cultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures);
        var cultureNames = cultures.Select(c => c.Name).ToList();

        // countries
        var countries = cultureNames.GroupBy(x => x.Substring(0, 2));
        var multiRegionCountries = countries.Where(x => x.Count() > 1)
            .Select(x => x.Key);
        cultureNames.AddRange(multiRegionCountries);

        cultureNames.Sort();
        return cultureNames;
    }

    public static IEnumerable<string> GetCultureNames() =>
        CultureInfos.Values.Select(x => x.Name);

    public static string GetIsoCurrencySymbol(string cultureName)
    {
        var cultureInfo = GetCulture(cultureName);
        return cultureInfo == null ? null : new RegionInfo(cultureName).ISOCurrencySymbol;
    }

    public static CultureInfo GetCulture(string cultureName) =>
        CultureInfos.ContainsKey(cultureName) ? CultureInfos[cultureName] : CultureInfo.CurrentCulture;
}