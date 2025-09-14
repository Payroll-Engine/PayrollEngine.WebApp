using System;
using System.Text;

namespace PayrollEngine.WebApp;

/// <summary>
/// Html tool
/// </summary>
public static class HtmlTool
{
    /// <summary>
    /// Build web link
    /// </summary>
    /// <param name="url">Link url</param>
    /// <param name="text">Link text (default: url)</param>
    /// <param name="blankTarget">Target blank link (default: rue)</param>
    /// <param name="noReferer">No referer link (default: true)</param>
    public static string BuildWebLink(string url, string text = null, bool blankTarget = true, bool noReferer = true)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            throw new ArgumentException(nameof(url));
        }

        var buffer = new StringBuilder();
        buffer.Append("<a ");
        if (blankTarget)
        {
            buffer.Append("target=\"_blank\" ");
        }
        if (noReferer)
        {
            // ReSharper disable StringLiteralTypo
            buffer.Append("rel=\"noopener noreferrer\" ");
            // ReSharper restore StringLiteralTypo
        }
        buffer.Append("href=\"");
        buffer.Append(url);
        buffer.Append("\">");

        buffer.Append(text ?? url);
        buffer.Append("</a>");

        return buffer.ToString();
    }

    /// <summary>
    /// Build mail-to link
    /// </summary>
    /// <param name="email">Email address</param>
    /// <param name="text">Email text</param>
    public static string BuildMailToLink(string email, string text = null)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException(nameof(email));
        }

        var buffer = new StringBuilder();
        buffer.Append("<a href=\"mailto:");
        buffer.Append(email);
        buffer.Append("\">");
        buffer.Append(text ?? email);
        buffer.Append("</a>");

        return buffer.ToString();
    }
}