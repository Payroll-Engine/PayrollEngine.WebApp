using MudBlazor;

namespace PayrollEngine.WebApp.Presentation;

/// <summary>
/// Global UI settings
/// </summary>
public static class Globals
{
    private static ComponentsConfiguration Configuration { get; set; } = new();

    /// <summary>
    /// Button variant
    /// </summary>
    public static Variant ButtonVariant => Configuration.ButtonVariant;

    /// <summary>
    /// Alternate button variant
    /// </summary>
    public static Variant ButtonAltVariant => Configuration.ButtonAltVariant;

    /// <summary>
    /// Toolbar button variant
    /// </summary>
    public static Variant ToolButtonVariant => Configuration.ToolButtonVariant;

    /// <summary>
    /// Tooltip delay in milliseconds
    /// </summary>
    public static double TooltipDelay => Configuration.TooltipDelay;

    /// <summary>
    /// Data grid rows per page
    /// </summary>
    public static int DataGridRowsPerPage => Configuration.DataGridRowsPerPage;

    public static void SetConfiguration(ComponentsConfiguration configuration)
    {
        if (configuration != null)
        {
            Configuration = configuration;
        }
    }
}