using MudBlazor.Utilities;

namespace PayrollEngine.WebApp.Presentation;

public static class ThemeServiceExtensions
{
    public static MudColor SelectedWorkingTypeColor(this IThemeService themeService) =>
        themeService.IsDarkMode ?
            themeService.Theme.PaletteDark.Primary :
            themeService.Theme.PaletteLight.Primary;
}