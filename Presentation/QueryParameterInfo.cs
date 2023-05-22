using System.Collections.Generic;

namespace PayrollEngine.WebApp.Presentation;

public class QueryParameterInfo
{
    public List<string> Keys { get; set; }
    public string Template { get; set; }
    public bool OptionalValue { get; set; }

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