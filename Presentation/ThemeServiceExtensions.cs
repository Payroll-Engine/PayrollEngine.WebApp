using MudBlazor.Utilities;

namespace PayrollEngine.WebApp.Presentation;

/// <summary>
/// Extension methods for <see cref="IThemeService" />
/// </summary>
public static class ThemeServiceExtensions
{
    /// <summary>
    /// Selected working type color
    /// </summary>
    /// <param name="themeService">Theme service</param>
    public static MudColor SelectedWorkingTypeColor(this IThemeService themeService) =>
        themeService.IsDarkMode ?
            themeService.Theme.PaletteDark.Primary :
            themeService.Theme.PaletteLight.Primary;
}