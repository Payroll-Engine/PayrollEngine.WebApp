namespace PayrollEngine.WebApp.Presentation.Regulation;

/// <summary>
/// Action item
/// </summary>
public class ActionItem
{
    /// <summary>
    /// The action index
    /// </summary>
    public int Index { get; set; }

    /// <summary>
    /// The action expression
    /// </summary>
    public string Action { get; set; }

    /// <summary>
    /// Default constructor
    /// </summary>
    public ActionItem()
    {
    }

    /// <summary>
    /// COpy constructor
    /// </summary>
    /// <param name="copySource">The copy source</param>
    public ActionItem(ActionItem copySource)
    {
        Index = copySource.Index;
        Action = copySource.Action;
    }

    /// <summary>
    /// Value constructor
    /// </summary>
    /// <param name="index">The action index</param>
    /// <param name="action">The action expression</param>
    public ActionItem(int index, string action = null)
    {
        Index = index;
        Action = action;
    }
}