namespace PayrollEngine.WebApp.Presentation.Regulation.Component;

/// <summary>
/// Item grid configuration
/// </summary>
public class ItemGridConfig
{
    /// <summary>
    /// Dense mode
    /// </summary>
    public bool DenseMode { get; set; }

    /// <summary>
    /// Items per page
    /// </summary>
    public int ItemsPageSize { get; } = 20;
}