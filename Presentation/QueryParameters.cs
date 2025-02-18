using System.Collections.Generic;

namespace PayrollEngine.WebApp.Presentation;

/// <summary>
/// Query parameters
/// </summary>
public static class QueryParameters
{
    /// <summary>
    /// Query parameters
    /// </summary>
    public static readonly List<QueryParameterInfo> Parameters =
    [
        new(["contains"], "contains({0},{1}) "),
        new(["not contains"], "not contains({0},{1}) "),
        new(["starts with"], "startswith({0},{1})"),
        new(["ends with"], "endswith({0},{1})"),
        new(["is empty"], "{0} eq null", true),
        new(["is not empty"], "{0} ne null", true),
        new(["equals", "=", "is"], "{0} eq {1}"),
        new(["not equals", "!=", "is not"], "{0} ne {1}"),
        new(["greater than", ">", "is after"], "{0} gt {1}"),
        new(["greater equals", ">=", "is on or after"], "{0} ge {1}"),
        new(["less than", "<", "is before"], "{0} lt {1}"),
        new(["less equals", "<=", "is on or before"], "{0} le {1}")
    ];
}