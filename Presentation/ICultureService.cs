using System.Collections.Generic;

namespace PayrollEngine.WebApp.Presentation;

/// <summary>
/// Culture service
/// </summary>
public interface ICultureService
{
    /// <summary>
    /// Get all cultures
    /// </summary>
    List<CultureDescription> GetCultures();

    /// <summary>
    /// Get culture
    /// </summary>
    /// <param name="cultureName">Culture name</param>
    CultureDescription GetCulture(string cultureName);

    /// <summary>
    /// Get ISO currency symbol
    /// </summary>
    /// <param name="cultureName">Culture name</param>
    string GetIsoCurrencySymbol(string cultureName);
}