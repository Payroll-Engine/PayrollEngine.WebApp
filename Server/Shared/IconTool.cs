using System;
using MudBlazor;
using PayrollEngine.WebApp.ViewModel;

namespace PayrollEngine.WebApp.Server.Shared;

public static class IconTool
{
    public static string GetCaseIcon(Case @case, string culture = null)
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
