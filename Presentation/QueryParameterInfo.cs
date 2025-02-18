using System.Collections.Generic;

namespace PayrollEngine.WebApp.Presentation;

/// <summary>
/// Query parameter info
/// </summary>
public class QueryParameterInfo
{
    /// <summary>
    /// Query keys
    /// </summary>
    public List<string> Keys { get; }

    /// <summary>
    /// Query template
    /// </summary>
    public string Template { get; }

    /// <summary>
    /// Optional query value
    /// </summary>
    public bool OptionalValue { get; }

    /// <summary>
    /// Default constructor
    /// </summary>
    public QueryParameterInfo()
    {
    }

    /// <summary>
    /// Value constructor
    /// </summary>
    /// <param name="keys">Query keys</param>
    /// <param name="template">Query template</param>
    /// <param name="optionalValue">Optional query value</param>
    public QueryParameterInfo(List<string> keys, string template, bool optionalValue = false)
    {
        Keys = keys;
        Template = template;
        OptionalValue = optionalValue;
    }
}