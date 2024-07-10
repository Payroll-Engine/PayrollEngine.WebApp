using MudBlazor;
using PayrollEngine.WebApp.Presentation;

namespace PayrollEngine.WebApp.Server.Shared;

public class ThemeService : IThemeService
{
    /// <inheritdoc />
    public bool IsDarkMode { get; set; }

    /// <inheritdoc />
    public MudTheme Theme => new()
    {
        PaletteLight = new PaletteLight
        {
            Primary = Colors.Gray.Darken4,
            Secondary = Colors.Gray.Darken3,
            AppbarText = Colors.Gray.Darken4,
            AppbarBackground = Colors.Gray.Lighten5
        },
        PaletteDark = new PaletteDark()
    };

    /// <inheritdoc />
    public Palette Palette => IsDarkMode ? Theme.PaletteDark : Theme.PaletteLight;
}