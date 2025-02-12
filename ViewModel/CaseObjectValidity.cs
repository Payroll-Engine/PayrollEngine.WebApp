using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PayrollEngine.WebApp.ViewModel;

public class CaseObjectValidity : IEquatable<CaseObjectValidity>
{
    /// <summary>
    /// If case object is valid or not
    /// </summary>
    public bool Valid => !rules.Any();

    /// <summary>
    /// List of all broken validation rules
    /// </summary>
    public IEnumerable<CaseObjectRule> Rules => rules;
    private readonly List<CaseObjectRule> rules = [];

    /// <summary>
    /// Validation rules (rule text only without case object name)
    /// </summary>
    public string ValidationRules => CreateRulesText();

    /// <summary>
    /// Validation rules with reference to case object it belongs to
    /// </summary>
    public string NamedValidationRules => CreateRulesText(true);

    public void AddRule(string caseObjectName, string ruleText) =>
        AddRule(new(caseObjectName, ruleText));

    private void AddRule(CaseObjectRule rule)
    {
        rules.Add(rule);
    }

    public void AddRules(IEnumerable<CaseObjectRule> objectRules)
    {
        rules.AddRange(objectRules);
    }

    private string CreateRulesText(bool includeObjectName = false)
    {
        // create lines of text with all broken validation rules
        var buffer = new StringBuilder();
        foreach (var rule in Rules)
        {
            var newRulesLine = $"{rule.RuleText}";
            if (includeObjectName)
            {
                newRulesLine = $"{rule.Name}: {newRulesLine}";
            }
            buffer.AppendLine(newRulesLine);
        }
        return buffer.ToString();
    }

    /// <summary>
    /// IEquatable implementation for comparision
    /// </summary>
    /// <param name="compare">Object to be compared with</param>
    /// <returns>Returns true if both are valid with all the same rules</returns>
    public bool Equals(CaseObjectValidity compare)
    {
        if (compare == null)
        {
            return false;
        }
        return Valid == compare.Valid &&
               Rules.SequenceEqual(compare.Rules);
    }

    public override string ToString() => $"{rules.Count} rules";
}