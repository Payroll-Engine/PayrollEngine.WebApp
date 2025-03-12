using MudBlazor;
using MudBlazor.Utilities;

namespace PayrollEngine.WebApp.Presentation;

/// <summary>
/// extension methods for <see cref="CasePriority"/>
/// </summary>
public static class CasePriorityExtensions
{
    /// <summary>
    /// Get case priority color
    /// </summary>
    /// <param name="priority">Priority</param>
    public static Color GetColor(this CasePriority priority) =>
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
    /// <param name="priority">Priority</param>
    /// <param name="themeService">Theme service</param>
    public static string GetStyle(this CasePriority priority, IThemeService themeService)
    {
        var backgroundColor = GetBackgroundColor(priority, themeService);
        var borderColor = GetBorderColor(priority, themeService);

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
    /// <param name="priority">Priority</param>
    /// <param name="themeService">Theme service</param>
    private static MudColor GetBackgroundColor(this CasePriority priority, IThemeService themeService)
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
    /// <param name="priority">Priority</param>
    /// <param name="themeService">Theme service</param>
    private static MudColor GetBorderColor(this CasePriority priority, IThemeService themeService)
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