namespace PayrollEngine.WebApp.ViewModel;

public class CaseObjectRule
{
    public string Name { get; set; }
    public string RuleText { get; set; }

    public CaseObjectRule(string name, string ruleText)
    {
        Name = name;
        RuleText = ruleText;
    }
}