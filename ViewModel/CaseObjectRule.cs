namespace PayrollEngine.WebApp.ViewModel;

/// <summary>
/// Case object rule
/// </summary>
/// <param name="caseField">Case field</param>
/// <param name="ruleText">Rule text</param>
public class CaseObjectRule(CaseFieldSet caseField, string ruleText)
{
    /// <summary>
    /// Case field
    /// </summary>
    public CaseFieldSet CaseField { get; } = caseField;

    /// <summary>
    /// Rule text
    /// </summary>
    public string RuleText { get; } = ruleText;

    /// <inheritdoc />
    public override string ToString() => $"{CaseField.Name} - {RuleText}";
}