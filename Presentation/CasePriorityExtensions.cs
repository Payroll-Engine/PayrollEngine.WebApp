using MudBlazor;
using MudBlazor.Utilities;

namespace PayrollEngine.WebApp.Presentation;

/// <summary>
/// extension methods for <see cref="CasePriority"/>
/// </summary>
public static class CasePriorityExtensions
{
    /// <param name="priority">Priority</param>
    extension(CasePriority priority)
    {
        /// <summary>
        /// Get case priority color
        /// </summary>
        public Color GetColor() =>
            priority switch
            {
                CasePriority.Low => Color.Dark,
                CasePriority.Normal => Color.Default,
                CasePriority.High => Color.Primary,
                CasePriority.Critical => Color.Error,
                _ => Color.Default
            };

        /// <summary>
        /// Get case priority html color
        /// </summary>
        /// <param name="themeService">Theme service</param>
        public string GetStyle(IThemeService themeService)
        {
            var backgroundColor = priority.GetBackgroundColor(themeService);
            var borderColor = priority.GetBorderColor(themeService);

            var borderWidth = 1;
            var borderStyle = "solid";
            switch (priority)
            {
                case CasePriority.Low:
                    borderStyle = "dotted";
                    break;
                case CasePriority.Normal:
                    break;
                case CasePriority.High:
                case CasePriority.Critical:
                    borderWidth = 2;
                    break;
            }

            return $"background-color: {backgroundColor};" +
                   $"border-color: {borderColor};" +
                   $"border-width: {borderWidth}px !important;" +
                   $"border-style: {borderStyle} !important;";
        }

        /// <summary>
        /// Get case priority background color
        /// </summary>
        /// <param name="themeService">Theme service</param>
        private MudColor GetBackgroundColor(IThemeService themeService)
        {
            return priority switch
            {
                CasePriority.Critical => themeService.IsDarkMode ?
                    themeService.Theme.PaletteDark.BackgroundGray :
                    themeService.Theme.PaletteLight.BackgroundGray,
                // all others
                _ => themeService.IsDarkMode ?
                    themeService.Theme.PaletteDark.Background :
                    themeService.Theme.PaletteLight.Background,
            };
        }

        /// <summary>
        /// Get case priority border color
        /// </summary>
        /// <param name="themeService">Theme service</param>
        private MudColor GetBorderColor(IThemeService themeService)
        {
            return priority switch
            {
                CasePriority.High => themeService.IsDarkMode ?
                    themeService.Theme.PaletteDark.Primary :
                    themeService.Theme.PaletteLight.Primary,
                CasePriority.Critical => themeService.IsDarkMode ?
                    themeService.Theme.PaletteDark.Error :
                    themeService.Theme.PaletteLight.Error,
                // low and normal
                _ => themeService.IsDarkMode ?
                    themeService.Theme.PaletteDark.GrayDefault :
                    themeService.Theme.PaletteLight.GrayDefault,
            };
        }
    }
}