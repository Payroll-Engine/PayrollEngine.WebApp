using System;
using System.Reflection;
using System.Collections.Generic;

namespace PayrollEngine.WebApp.Presentation;

/// <summary>
/// extension methods for <see cref="Type" />
/// </summary>
public static class TypeExtensions
{

    #region Localizations

    /// <summary>
    /// Test if localization property exists
    /// </summary>
    /// <param name="type">The object type</param>
    /// <param name="propertyName">The property name</param>
    /// <returns>A hash set with ordinal ignore case</returns>
    public static bool IsLocalizable(this Type type, string propertyName) =>
        type.ContainsProperty(GetLocalizationsPropertyName(propertyName), typeof(Dictionary<string, string>));

    /// <summary>
    /// Get the property localization values
    /// </summary>
    /// <param name="source">The source object</param>
    /// <param name="propertyName">The property name</param>
    /// <returns>A dictionary with the object attributes</returns>
    public static Dictionary<string, string> GetLocalizations(this object source, string propertyName) =>
        source.GetPropertyValue<Dictionary<string, string>>(GetLocalizationsPropertyName(propertyName));

    /// <summary>
    /// Get the localization property
    /// </summary>
    /// <param name="type">The source type</param>
    /// <param name="propertyName">the source property</param>
    /// <returns>The localization property</returns>
    /// <exception cref="PayrollException"></exception>
    public static PropertyInfo GetLocalizationsProperty(this Type type, string propertyName)
    {
        if (string.IsNullOrWhiteSpace(propertyName))
        {
            throw new ArgumentException(nameof(propertyName));
        }

        // source property
        var property = type.GetProperty(propertyName);
        if (property == null)
        {
            throw new PayrollException($"Invalid localization source property {propertyName}.");
        }

        // localizations property
        var localizationPropertyName = GetLocalizationsPropertyName(propertyName);
        var localizationProperty = type.GetProperty(localizationPropertyName);
        if (localizationProperty == null)
        {
            throw new PayrollException($"Missing localization property {localizationPropertyName}.");
        }
        if (localizationProperty.PropertyType != typeof(Dictionary<string, string>))
        {
            throw new PayrollException($"Localization property {localizationPropertyName} must be a Dictionary<string,string>.");
        }

        return localizationProperty;
    }

    private static string GetLocalizationsPropertyName(string propertyName) =>
        $"{propertyName}Localizations";

    #endregion

}