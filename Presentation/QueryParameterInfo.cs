using System.Collections.Generic;

namespace PayrollEngine.WebApp.Presentation;

public class QueryParameterInfo
{
    public List<string> Keys { get; }
    public string Template { get; }
    public bool OptionalValue { get; }

    public QueryParameterInfo()
    {
    }

    public QueryParameterInfo(List<string> keys, string template, bool optionalValue = false)
    {
        Keys = keys;
        Template = template;
        OptionalValue = optionalValue;
    }
}