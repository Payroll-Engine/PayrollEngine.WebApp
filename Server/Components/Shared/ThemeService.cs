using MudBlazor;
using PayrollEngine.WebApp.Presentation;

namespace PayrollEngine.WebApp.Server.Components.Shared;

/// <summary>
/// Theme service managing dark mode and MudBlazor theme palettes
/// </summary>
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

    /// <inheritdoc />
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

}