using MudBlazor.Utilities;

namespace PayrollEngine.WebApp.Presentation;

/// <summary>
/// Extension methods for <see cref="IThemeService" />
/// </summary>
public static class ThemeServiceExtensions
{
    /// <param name="themeService">Theme service</param>
    extension(IThemeService themeService)
    {
        /// <summary>
        /// Selected background color
        /// </summary>
        public MudColor SelectedBackgroundColor() =>
            themeService.IsDarkMode ?
                themeService.Theme.PaletteDark.Primary:
                themeService.Theme.PaletteLight.GrayLighter;
    }
}