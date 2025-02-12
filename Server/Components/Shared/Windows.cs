using System.Runtime.Versioning;
using Microsoft.Win32;

namespace PayrollEngine.WebApp.Server.Components.Shared;

internal static class Windows
{
    // dark theme registry settings
    private const string RegistryKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";
    private const string RegistryValueName = "AppsUseLightTheme";

    /// <summary>
    /// Get the windows dark mode setting
    /// </summary>
    [SupportedOSPlatform("windows")]
    internal static bool GetDarkMode()
    {
        using var key = Registry.CurrentUser.OpenSubKey(RegistryKeyPath);
        var registryValue = key?.GetValue(RegistryValueName);
        if (registryValue == null)
        {
            return false;
        }
        return (int)registryValue <= 0;
    }

}