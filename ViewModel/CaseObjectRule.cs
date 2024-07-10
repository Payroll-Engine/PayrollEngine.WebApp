namespace PayrollEngine.WebApp.ViewModel;

public class CaseObjectRule(string name, string ruleText)
{
    public string Name { get; } = name;
    public string RuleText { get; } = ruleText;
}