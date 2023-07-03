using Microsoft.Extensions.Localization;

namespace PayrollEngine.WebApp.Shared;

public class DataGridLocalizer : LocalizerBase
{
    private readonly SharedLocalizer sharedLocalizer;
    private readonly DialogLocalizer dialogLocalizer;

    public DataGridLocalizer(IStringLocalizerFactory factory) :
        base(factory)
    {
        sharedLocalizer = new(factory);
        dialogLocalizer = new(factory);
    }

    public string SymbolNotEquals => PropertyValue();
    public string SymbolLess => PropertyValue();
    public string SymbolLessEquals => PropertyValue();
    public string SymbolEquals => PropertyValue();
    public string SymbolGreater => PropertyValue();
    public string SymbolGreaterEquals => PropertyValue();

    public string AddFilter => PropertyValue();
    public string Apply => PropertyValue();
    public string Cancel => dialogLocalizer.Cancel;
    public string Clear => PropertyValue();
    public string CollapseAllGroups => PropertyValue();
    public string Column => PropertyValue();
    public string Columns => PropertyValue();
    public string Contains => PropertyValue();
    public string EndsWith => PropertyValue();
    public new string Equals => PropertyValue();
    public string ExpandAllGroups => PropertyValue();
    public string False => PropertyValue();
    public string Filter => PropertyValue();
    public string FilterValue => PropertyValue();
    public string Group => PropertyValue();
    public string Hide => PropertyValue();
    public string HideAll => PropertyValue();
    public string Is => PropertyValue();
    public string IsAfter => PropertyValue();
    public string IsBefore => PropertyValue();
    public string IsEmpty => PropertyValue();
    public string IsNot => PropertyValue();
    public string IsNotEmpty => PropertyValue();
    public string IsOnOrAfter => PropertyValue();
    public string IsOnOrBefore => PropertyValue();
    public string NotContains => PropertyValue();
    public string NotEquals => PropertyValue();
    public string Operator => PropertyValue();
    public string RefreshData => PropertyValue();
    public string Save =>  dialogLocalizer.Save;
    public string ShowAll => PropertyValue();
    public string StartsWith => PropertyValue();
    public string True => PropertyValue();
    public string Ungroup => PropertyValue();
    public string Unsort => PropertyValue();
    public string Value => sharedLocalizer.Value;

    // data grid pager
    public string PagerInfoFormat => PropertyValue();
    public string PagerRowsPerPage => PropertyValue();

}