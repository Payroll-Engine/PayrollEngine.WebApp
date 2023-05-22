using MudBlazor;
using PayrollEngine.WebApp.Presentation;

namespace PayrollEngine.WebApp.Server.Shared
{
    public class ThemeService : IThemeService
    {
        /// <inheritdoc />
        public bool IsDarkMode { get; set; }

        /// <inheritdoc />
        public MudTheme Theme => new()
        {
            Palette = new PaletteLight
            {
                Primary = Colors.Grey.Darken4,
                Secondary = Colors.Grey.Darken3,
                AppbarText = Colors.Grey.Darken4,
                AppbarBackground = Colors.Grey.Lighten5,
            },
            PaletteDark = new PaletteDark()
        };

#pragma warning disable CS0618
        /// <inheritdoc />
        public Palette Palette => IsDarkMode ? Theme.PaletteDark : Theme.Palette;
#pragma warning restore CS0618
    }
}
