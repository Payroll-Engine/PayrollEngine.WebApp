using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace PayrollEngine.WebApp;

public static class StringExtensions
{
    public static string LastStringAfterComma(this string input) => input.Split('.').ToList().Last();

    public static string FirstCharacterToLower(this string value) =>
        char.ToLowerInvariant(value[0]) + value.Substring(1);

    public static string FirstCharacterToUpper(this string value) =>
        value.First().ToString().ToUpper() + value.Substring(1);

    public static string AddSpaceBeforeUppercase(this string value)
    {
        var result = string.Empty;
        foreach (var c in value)
        {
            if (char.IsUpper(c))
            {
                result += ' ';
            }

            result += c;
        }

        return result;
    }

    public static bool ToEnum<TEnum>(this string value, out TEnum result, bool ignoreCase = true)
    {
        result = default;
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }
        if (!Enum.TryParse(typeof(TEnum), value, ignoreCase, out var valueResult))
        {
            return false;
        }

        result = (TEnum)valueResult;
        return true;
    }

    public static string ToPropertyName(this string propertyName)
    {
        if (string.IsNullOrWhiteSpace(propertyName))
        {
            return null;
        }
        return propertyName
            .Replace('.', '_')
            .Replace(':', '_')
            .Replace('$', '_')
            .Replace('?', '_');
    }

    private const string CaptionDelimiter = "...";
    /// <summary>
    /// Gets the string as a caption with a delimited length
    /// </summary>
    /// <param name="text">The caption text</param>
    /// <param name="maxLength">The maximum caption length</param>
    /// <returns>The caption</returns>
    public static string GetCaption(this string text, int maxLength = 30)
    {
        if (text == null)
        {
            return null;
        }
        if (text.Length > maxLength - CaptionDelimiter.Length)
        {
            return text.Substring(0, maxLength - CaptionDelimiter.Length) + CaptionDelimiter;
        }
        return text;
    }

    /// <summary>Removes all hyperlinks</summary>
    /// <param name="text">The html text</param>
    /// <param name="format">The replacement format</param>
    /// <returns>The html text without hyperlinks</returns>
    public static string RemoveLinks(this string text, string format = "[{0}]")
    {
        if (text == null)
        {
            return null;
        }

        var hyperlinks = ExtractLinks(text);
        foreach (var hyperlink in hyperlinks)
        {
            var replaceText = string.IsNullOrWhiteSpace(format) ?
                string.Empty : string.Format(format, hyperlink.Item3);
            text = text.Replace(hyperlink.Item1, replaceText);
        }
        return text;
    }

    /// <summary>Extract all hyperlinks</summary>
    /// <param name="text">The html text</param>
    /// <returns>A list with the a-tag, the url and the content</returns>
    public static List<Tuple<string, string, string>> ExtractLinks(this string text)
    {
        var hyperlinks = new List<Tuple<string, string, string>>();
        if (text == null)
        {
            return hyperlinks;
        }
        var hrefPattern = @"href\s*=\s*(?:[""'](?<1>[^""']*)[""']|(?<1>[^>\s]+))";
        var aTagRegex = new Regex(@"<a.*?>(.*?)</a>");
        foreach (Match aTagMatch in aTagRegex.Matches(text))
        {
            if (aTagMatch.Groups.Count >= 2)
            {
                var aTag = aTagMatch.Groups[0].Value;
                Match hrefMatch = Regex.Match(aTag, hrefPattern);
                if (!hrefMatch.Success)
                {
                    continue;
                }
                hyperlinks.Add(new(aTag, hrefMatch.Groups[1].Value, aTagMatch.Groups[1].Value));
            }
        }
        return hyperlinks;
    }
}