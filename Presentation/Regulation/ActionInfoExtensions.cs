using System.Linq;
using System.Text;
using PayrollEngine.Client.Model;

namespace PayrollEngine.WebApp.Presentation.Regulation;

/// <summary>
/// extension methods for <see cref="ActionInfo"/>
/// </summary>
public static class ActionInfoExtensions
{
    /// <summary>
    /// Get the expression template
    /// </summary>
    public static string GetExpressionTemplate(this ActionInfo actionInfo)
    {
        if (actionInfo == null)
        {
            return null;
        }
        if (actionInfo.Parameters == null || !actionInfo.Parameters.Any())
        {
            return actionInfo.Name;
        }

        // build expression
        var buffer = new StringBuilder();
        buffer.Append(actionInfo.Name);
        buffer.Append('(');
        foreach (var parameter in actionInfo.Parameters)
        {
            buffer.Append(parameter.Name);
            if (actionInfo.Parameters.Last() != parameter)
            {
                buffer.Append(", ");
            }
        }
        buffer.Append(')');
        return buffer.ToString();
    }
}