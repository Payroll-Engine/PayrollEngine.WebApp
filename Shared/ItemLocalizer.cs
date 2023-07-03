using Microsoft.Extensions.Localization;

namespace PayrollEngine.WebApp.Shared;

public class ItemLocalizer : LocalizerBase
{
    public ItemLocalizer(IStringLocalizerFactory factory) :
        base(factory)
    {
    }

    public string CSharpExpression => PropertyValue();
    public string BaseField => PropertyValue();
    public string InitOnlyField => PropertyValue();
    public string ReadOnlyField => PropertyValue();

    // add/create
    public string Add => PropertyValue();
    public string Added(string item) =>
        FormatValue(PropertyValue(), nameof(item), item);
    public string AddTitle(string item) =>
        FormatValue(PropertyValue(), nameof(item), item);
    public string AddHelp(string item) =>
        FormatValue(PropertyValue(), nameof(item), item);

    public string NotAvailable(string item) =>
        FormatValue(PropertyValue(), nameof(item), item);
    public string SelectParent(string item) =>
        FormatValue(PropertyValue(), nameof(item), item);

    // edit/updated
    public string Edit => PropertyValue();
    public string EditTitle(string item) =>
        FormatValue(PropertyValue(), nameof(item), item);
    public string Updated(string item) =>
        FormatValue(PropertyValue(), nameof(item), item);
    public string EditHelp(string item) =>
        FormatValue(PropertyValue(), nameof(item), item);

    // delete
    public string Delete => PropertyValue();
    public string Deleted(string item) =>
        FormatValue(PropertyValue(), nameof(item), item);
    public string DeleteQuery(string item) =>
        FormatValue(PropertyValue(), nameof(item), item);
    public string DeleteTitle(string item) =>
        FormatValue(PropertyValue(), nameof(item), item);
    public string DeleteHelp(string item) =>
        FormatValue(PropertyValue(), nameof(item), item);

    // remove
    public string Remove => PropertyValue();
    public string RemoveTitle(string item) =>
        FormatValue(PropertyValue(), nameof(item), item);
    public string Removed(string item) =>
        FormatValue(PropertyValue(), nameof(item), item);
    public string RemoveAll => PropertyValue();

    // save
    public string SaveHelp(string item) =>
        FormatValue(PropertyValue(), nameof(item), item);
    public string DeriveHelp(string item) =>
        FormatValue(PropertyValue(), nameof(item), item);
}