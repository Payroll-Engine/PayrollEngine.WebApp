using System.Linq;
using System.Text;
using PayrollEngine.Client.Model;

namespace PayrollEngine.WebApp.ViewModel;

public static class CaseChangeExtensions
{
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