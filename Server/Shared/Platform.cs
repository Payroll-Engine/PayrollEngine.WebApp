using System.Runtime.InteropServices;

namespace PayrollEngine.WebApp.Server.Shared;

internal static class Platform
{
    /// <summary>
    /// Get the system dark mode setting (default: false)
    /// </summary>
    internal static bool GetDarkMode()
    {
        var darkMode = false;
        // windows
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            darkMode = Windows.GetDarkMode();
        }
        return darkMode;
    }
}
