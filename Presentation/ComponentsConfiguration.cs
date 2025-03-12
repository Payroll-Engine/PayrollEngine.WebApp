using MudBlazor;
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace PayrollEngine.WebApp.Presentation;

/// <summary>
/// Components configuration
/// </summary>
public class ComponentsConfiguration
{
    /// <summary>
    /// Button variant (default: filled)
    /// </summary>
    public Variant ButtonVariant { get; set; } = Variant.Filled;

    /// <summary>
    /// Alternate button variant (default: outlined)
    /// </summary>
    public Variant ButtonAltVariant { get; set; } = Variant.Filled;

    /// <summary>
    /// Toolbar button variant (default: outlined)
    /// </summary>
    public Variant ToolButtonVariant { get; set; } = Variant.Filled;

    /// <summary>
    /// Tooltip delay in milliseconds (default: 1000)
    /// </summary>
    public double TooltipDelay { get; set; } = 1000;

    /// <summary>
    /// Data grid rows per page (default: 25)
    /// </summary>
    public int DataGridRowsPerPage { get; set; } = 25;
}