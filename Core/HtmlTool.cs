using System;
using System.Text;

namespace PayrollEngine.WebApp;

public static class HtmlTool
{
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
            buffer.Append("rel=\"noopener noreferrer\" ");
        }
        buffer.Append("href=\"");
        buffer.Append(url);
        buffer.Append("\">");

        buffer.Append(text ?? url);
        buffer.Append("</a>");

        return buffer.ToString();
    }

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