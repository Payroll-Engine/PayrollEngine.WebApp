using MudBlazor;
using PayrollEngine.WebApp.Presentation;

namespace PayrollEngine.WebApp.Server.Components.Shared;

public class ThemeService : IThemeService
{
    /// <inheritdoc />
    public bool IsDarkMode
    {
        get;
        set
        {
            if (value == field)
            {
                return;
            }

            field = value;
            // event
            DarkModeChanged?.InvokeAsync(this, field);
        }
    }

    public AsyncEvent<bool> DarkModeChanged { get; set; }

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