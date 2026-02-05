
namespace PayrollEngine.WebApp.Presentation.Regulation;

/// <summary>
/// Regulation action category
/// </summary>
public class ActionCategory
{
    /// <summary>
    /// Category name
    /// </summary>
    public string Name { get; init; }

    /// <summary>
    /// Category label
    /// </summary>
    public string Label { get; init; }

    /// <summary>
    /// Display order
    /// </summary>
    public int DisplayOrder { get; init; }

    /// <inheritdoc/>
    public override string ToString() => Name;
}