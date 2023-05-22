using System.Collections.Generic;

namespace PayrollEngine.WebApp.Presentation;

public static class QueryParameters
{
    public static readonly List<QueryParameterInfo> Parameters = new()
    {
        { new(new() { "contains" }, "contains({0},{1}) ") },
        { new(new() { "not contains" }, "not contains({0},{1}) ") },
        { new(new() { "starts with" }, "startswith({0},{1})") },
        { new(new() { "ends with" }, "endswith({0},{1})") },
        { new(new() { "is empty" }, "{0} eq null", true) },
        { new(new() { "is not empty" }, "{0} ne null", true) },
        { new(new() { "equals", "=", "is" }, "{0} eq {1}") },
        { new(new() { "not equals", "!=", "is not" }, "{0} ne {1}") },
        { new(new() { "greater than", ">", "is after" }, "{0} gt {1}") },
        { new(new() { "greater equals", ">=", "is on or after" }, "{0} ge {1}") },
        { new(new() { "less than", "<", "is before" }, "{0} lt {1}") },
        { new(new() { "less equals", "<=", "is on or before" }, "{0} le {1}") }
    };
}