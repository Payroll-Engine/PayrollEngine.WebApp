namespace PayrollEngine.WebApp.Presentation;

/// <summary>
/// OData definitions
/// see https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.odata.query.allowedlogicaloperators?view=odata-aspnetcore-8.0
/// </summary>
public static class ODataParameters
{
    public static readonly string Ascending = "asc";
    public static readonly string Descending = "desc";

    public static readonly string And = "and";
    public static readonly string Or = "or";
    public static readonly string Not = "not";

    public static readonly string Has = "has";
    public static readonly string All = "all";
    public static readonly string None = "None";

    public static readonly string Equal = "equal";
    public static readonly string NotEqual = "notequal";
    public static readonly string GreaterThan = "greaterthan";
    public static readonly string GreaterThanOrEqual = "greaterthanorequal";
    public static readonly string LessThan = "lessthan";
    public static readonly string LessThanOrEqual = "lessthanorequal";
}