using Microsoft.Extensions.Localization;

namespace PayrollEngine.WebApp.Shared;

public class DataGridLocalizer : LocalizerBase
{
    public DataGridLocalizer(IStringLocalizerFactory factory) :
        base(factory)
    {
    }

    public string SymbolNotEquals => FromCaller();
    public string SymbolLess => FromCaller();
    public string SymbolLessEquals => FromCaller();
    public string SymbolEquals => FromCaller();
    public string SymbolGreater => FromCaller();
    public string SymbolGreaterEquals => FromCaller();

    public string AddFilter => FromCaller();
    public string Apply => FromCaller();
    public string Cancel => FromCaller();
    public string Clear => FromCaller();
    public string CollapseAllGroups => FromCaller();
    public string Column => FromCaller();
    public string Columns => FromCaller();
    public string Contains => FromCaller();
    public string EndsWith => FromCaller();
    public new string Equals => FromCaller();
    public string ExpandAllGroups => FromCaller();
    public string False => FromCaller();
    public string Filter => FromCaller();
    public string FilterValue => FromCaller();
    public string Group => FromCaller();
    public string Hide => FromCaller();
    public string HideAll => FromCaller();
    public string Is => FromCaller();
    public string IsAfter => FromCaller();
    public string IsBefore => FromCaller();
    public string IsEmpty => FromCaller();
    public string IsNot => FromCaller();
    public string IsNotEmpty => FromCaller();
    public string IsOnOrAfter => FromCaller();
    public string IsOnOrBefore => FromCaller();
    public string NotContains => FromCaller();
    public string NotEquals => FromCaller();
    public string Operator => FromCaller();
    public string RefreshData => FromCaller();
    public string Save => FromCaller();
    public string ShowAll => FromCaller();
    public string StartsWith => FromCaller();
    public string True => FromCaller();
    public string Ungroup => FromCaller();
    public string Unsort => FromCaller();
    public string Value => FromCaller();
}