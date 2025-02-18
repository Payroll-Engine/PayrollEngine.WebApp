using System.Linq;
using System.Text;
using PayrollEngine.Client.Model;

namespace PayrollEngine.WebApp.ViewModel;

/// <summary>
/// Extension methods for <see cref="ICaseChange"/>
/// </summary>
public static class CaseChangeExtensions
{
    /// <summary>
    /// Get case issues
    /// </summary>
    /// <param name="caseChange">Case change</param>
    public static string GetCaseIssues(this ICaseChange caseChange)
    {
        if (caseChange.Issues == null || !caseChange.Issues.Any())
        {
            return null;
        }

        var buffer = new StringBuilder();
        foreach (var issue in caseChange.Issues)
        {
            if (buffer.Length > 0)
            {
                buffer.Append("<br />");
            }
            buffer.Append(string.IsNullOrWhiteSpace(issue.CaseFieldName) ?
                $"Error in case {issue.CaseName}:<br />" :
                $"Error in field {issue.CaseFieldName} of case {issue.CaseName}:<br />");
            buffer.Append($"- {issue.Message}");
        }
        return buffer.ToString();
    }
}