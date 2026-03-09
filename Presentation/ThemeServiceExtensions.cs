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
        /// Primary text color based on dark mode
        /// </summary>
        public MudColor TextPrimaryColor() =>
            themeService.IsDarkMode ?
                themeService.Theme.PaletteDark.TextPrimary :
                themeService.Theme.PaletteLight.TextPrimary;

        /// <summary>
        /// Background gray color based on dark mode
        /// </summary>
        public MudColor BackgroundGrayColor() =>
            themeService.IsDarkMode ?
                themeService.Theme.PaletteDark.BackgroundGray :
                themeService.Theme.PaletteLight.BackgroundGray;

        /// <summary>
        /// Selected background color
        /// </summary>
        public MudColor SelectedBackgroundColor() =>
            themeService.IsDarkMode ?
                themeService.Theme.PaletteDark.Primary :
                themeService.Theme.PaletteLight.GrayLighter;
    }
}