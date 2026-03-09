using MudBlazor;

namespace PayrollEngine.WebApp.Presentation;

/// <summary>Theme service</summary>
public interface IThemeService
{
    /// <summary>True for the dark mode</summary>
    bool IsDarkMode { get; set; }

    /// <summary>Dark mode changed handler</summary>
    AsyncEvent<bool> DarkModeChanged { get; set; }

    /// <summary>The current theme</summary>
    MudTheme Theme { get; }

}