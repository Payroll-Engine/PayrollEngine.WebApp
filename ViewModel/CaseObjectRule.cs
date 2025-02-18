namespace PayrollEngine.WebApp.ViewModel;

/// <summary>
/// Case object rule
/// </summary>
/// <param name="name">Rule name</param>
/// <param name="ruleText">Rule text</param>
public class CaseObjectRule(string name, string ruleText)
{
    /// <summary>
    /// Rule name
    /// </summary>
    public string Name { get; } = name;

    /// <summary>
    /// Rule text
    /// </summary>
    public string RuleText { get; } = ruleText;

    /// <inheritdoc />
    public override string ToString() => $"{Name} - {RuleText}";
}