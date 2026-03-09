using System;
using System.Globalization;
using MudBlazor;
using PayrollEngine.WebApp.ViewModel;

namespace PayrollEngine.WebApp.Server.Components.Shared;

/// <summary>
/// Tool for resolving MudBlazor icon names from case attributes
/// </summary>
public static class IconTool
{
    /// <summary>
    /// Get the icon for a case from its attributes
    /// </summary>
    /// <param name="case">The case</param>
    /// <param name="culture">The culture for localized attributes</param>
    /// <returns>The icon SVG string, or null if not found</returns>
    public static string GetCaseIcon(Case @case, CultureInfo culture)
    {
        var icon = @case.Attributes.GetIcon(culture);
        if (string.IsNullOrWhiteSpace(icon))
        {
            return null;
        }

        // material
        var typeIcon = GetMaterialIconName(icon);
        if (!string.IsNullOrWhiteSpace(typeIcon))
        {
            return typeIcon;
        }

        // custom
        typeIcon = GetTypeIconName(typeof(Icons.Custom), icon);
        if (!string.IsNullOrWhiteSpace(typeIcon))
        {
            return typeIcon;
        }

        return null;
    }

    private static string GetMaterialIconName(string name)
    {
        var typeIcon = GetTypeIconName(typeof(Icons.Material.Filled), name);
        if (!string.IsNullOrWhiteSpace(typeIcon))
        {
            return typeIcon;
        }

        typeIcon = GetTypeIconName(typeof(Icons.Material.Outlined), name);
        if (!string.IsNullOrWhiteSpace(typeIcon))
        {
            return typeIcon;
        }

        typeIcon = GetTypeIconName(typeof(Icons.Material.Rounded), name);
        if (!string.IsNullOrWhiteSpace(typeIcon))
        {
            return typeIcon;
        }

        typeIcon = GetTypeIconName(typeof(Icons.Material.Sharp), name);
        if (!string.IsNullOrWhiteSpace(typeIcon))
        {
            return typeIcon;
        }

        typeIcon = GetTypeIconName(typeof(Icons.Material.TwoTone), name);
        if (!string.IsNullOrWhiteSpace(typeIcon))
        {
            return typeIcon;
        }
        return null;
    }

    private static string GetTypeIconName(Type type, string name)
    {
        var typeName = $"{type.Name}.";
        if (!name.StartsWith(typeName))
        {
            return null;
        }

        var field = type.GetField(name.RemoveFromStart(typeName));
        if (field != null)
        {
            return field.GetValue(null) as string;
        }
        return null;
    }
}
