namespace PayrollEngine.WebApp.ViewModel;

public class CaseObjectRule
{
    public string Name { get; }
    public string RuleText { get; }

    public CaseObjectRule(string name, string ruleText)
    {
        Name = name;
        RuleText = ruleText;
    }
}