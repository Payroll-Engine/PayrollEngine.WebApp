using System;
using System.Linq;
using System.Globalization;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace PayrollEngine.WebApp.Presentation;

/// <inheritdoc />
public class CultureService : ICultureService
{
    private AppConfiguration AppConfiguration { get; }
    private readonly IDictionary<string, CultureDescription> Cultures =
        new Dictionary<string, CultureDescription>();

    /// <summary>
    /// default constructor, preload all cultures
    /// </summary>
    public CultureService(IConfiguration configuration)
    {
        AppConfiguration = configuration.GetConfiguration<AppConfiguration>();
        SetupCultures();
    }

    /// <inheritdoc />
    public List<CultureDescription> GetCultures()
    {
        var cultures = new List<CultureDescription>();
        var preferredCultures = new Dictionary<int, CultureDescription>();
        var allCultures = Cultures.Values.OrderBy(x => x.Name);

        // preferred culture order
        if (AppConfiguration.PreferredCultures.Any())
        {
            foreach (var cultureInfo in allCultures)
            {
                var preferredIndex = -1;
                for (var i = 0; i < AppConfiguration.PreferredCultures.Count; i++)
                {
                    var preferredCulture = AppConfiguration.PreferredCultures[i];
                    if (preferredCulture.Contains('-'))
                    {
                        // exact matching name
                        if (string.Equals(preferredCulture, cultureInfo.Name))
                        {
                            preferredIndex = i;
                            break;
                        }
                    }
                    else
                    {
                        // wildcard matching name
                        if (cultureInfo.Name.StartsWith(preferredCulture))
                        {
                            preferredIndex = i;
                            break;
                        }
                    }
                }
                if (preferredIndex >= 0)
                {
                    preferredCultures.Add(preferredIndex, new(cultureInfo.CultureInfo));
                }
                else
                {
                    cultures.Add(new(cultureInfo.CultureInfo));
                }
            }

            // add preferred cultures in reverse ordered to keep the configuration order
            foreach (var i in preferredCultures.Keys.OrderByDescending(x => x))
            {
                cultures.Insert(0, preferredCultures[i]);
            }
        }
        else
        {
            // no preferred cultures
            cultures.AddRange(allCultures);
        }
        return cultures;
    }

    /// <inheritdoc />
    public CultureDescription GetCulture(string cultureName) =>
        Cultures.Values.FirstOrDefault(x => string.Equals(x.Name, cultureName));

    /// <inheritdoc />
    public string GetIsoCurrencySymbol(string cultureName)
    {
        if (string.IsNullOrWhiteSpace(cultureName))
        {
            throw new ArgumentException(nameof(cultureName));
        }
        var cultureInfo = GetCultureInfo(cultureName);
        return cultureInfo == null ? null : new RegionInfo(cultureName).ISOCurrencySymbol;
    }

    private void SetupCultures()
    {
        Cultures.Clear();

        var allCultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
        foreach (var cultureInfo in allCultures)
        {
            if (cultureInfo.IsNeutralCulture || string.IsNullOrWhiteSpace(cultureInfo.Name))
            {
                continue;
            }
            Cultures.Add(cultureInfo.Name, new(cultureInfo));
        }
    }

    /// <summary>
    /// Get culture info by name
    /// </summary>
    /// <param name="cultureName">Culture name</param>
    private CultureInfo GetCultureInfo(string cultureName)
    {
        if (string.IsNullOrWhiteSpace(cultureName))
        {
            return CultureInfo.CurrentCulture;
        }

        // matching culture
        if (Cultures.TryGetValue(cultureName, out var culture))
        {
            return culture.CultureInfo;
        }

        // base culture
        var index = cultureName.IndexOf('-');
        if (index > 0)
        {
            var baseCultureName = cultureName.Substring(0, index);
            if (Cultures.TryGetValue(baseCultureName, out var baseCulture))
            {
                return baseCulture.CultureInfo;
            }
        }

        return CultureInfo.CurrentCulture;
    }
}