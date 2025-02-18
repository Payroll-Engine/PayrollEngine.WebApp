using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using PayrollEngine.Client.Model;

namespace PayrollEngine.WebApp.ViewModel;

/// <summary>
/// View model task
/// </summary>
public class Task : Client.Model.Task, IViewModel,
    IViewAttributeObject, IKeyEquatable<Task>
{
    /// <summary>
    /// Default constructor
    /// </summary>
    public Task()
    {
    }

    /// <summary>
    /// Copy constructor
    /// </summary>
    /// <param name="copySource">Copy source</param>
    public Task(Task copySource) :
        base(copySource)
    {
    }

    /// <summary>
    /// Base model constructor
    /// </summary>
    /// <param name="copySource">Copy source</param>
    public Task(Client.Model.Task copySource) :
        base(copySource)
    {
    }

    /// <summary>
    /// Schedule date
    /// </summary>
    public new DateTime? Scheduled
    {
        get => IsScheduled ? base.Scheduled : null;
        set => base.Scheduled = value ?? DateTime.MinValue;
    }

    /// <summary>
    /// Test for scheduled task
    /// </summary>
    public bool IsScheduled => base.Scheduled != DateTime.MinValue;

    /// <summary>
    /// Get the localized name
    /// </summary>
    /// <param name="culture">Culture</param>
    public string GetLocalizedName(string culture) =>
        culture.GetLocalization(NameLocalizations, Name);

    /// <summary>
    /// Instruction text
    /// </summary>
    public string InstructionText =>
        RemoveLinks(Instruction);

    /// <summary>
    /// Instruction links
    /// </summary>
    public List<Tuple<string, string, string>> InstructionLinks =>
        ExtractLinks(Instruction);

    #region Attributes

    /// <inheritdoc />
    public string GetStringAttribute(string name) =>
        Attributes?.GetStringAttributeValue(name);

    /// <inheritdoc />
    public decimal GetNumericAttribute(string name) =>
        Attributes?.GetDecimalAttributeValue(name) ?? 0;

    /// <inheritdoc />
    public bool GetBooleanAttribute(string name) =>
        Attributes?.GetBooleanAttributeValue(name) ?? false;

    #endregion

    #region String

    /// <summary>Removes all hyperlinks</summary>
    /// <param name="text">The html text</param>
    /// <param name="format">The replacement format</param>
    /// <returns>The html text without hyperlinks</returns>
    private static string RemoveLinks(string text, string format = "[{0}]")
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
    private static List<Tuple<string, string, string>> ExtractLinks(string text)
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

    #endregion

    /// <summary>Compare two objects</summary>
    /// <param name="compare">The object to compare with this</param>
    /// <returns>True for objects with the same data</returns>
    public bool Equals(Task compare) =>
        base.Equals(compare);

    /// <summary>Compare two objects</summary>
    /// <param name="compare">The object to compare with this</param>
    /// <returns>True for objects with the same data</returns>
    public bool Equals(IViewModel compare) =>
        Equals(compare as Task);

    /// <inheritdoc />
    public bool EqualKey(Task compare) => false;
}