using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using System.Globalization;

namespace PayrollEngine.WebApp.Shared;

public class ItemLocalizer(IStringLocalizerFactory factory, CultureInfo culture) : 
    LocalizerBase(factory, culture: culture)
{
    public string CSharpExpression => PropertyValue();
    public string BaseField => PropertyValue();
    public string InitOnlyField => PropertyValue();
    public string ReadOnlyField => PropertyValue();

    public string NotAvailable(string item) =>
        FormatValue(PropertyValue(), nameof(item), item);
    public string SelectParent(string item) =>
        FormatValue(PropertyValue(), nameof(item), item);

    // create
    public string Create => PropertyValue();
    public string Created(string item) =>
        FormatValue(PropertyValue(), nameof(item), item);
    public string CreateTitle(string item) =>
        FormatValue(PropertyValue(), nameof(item), item);
    public string CreateHelp(string item) =>
        FormatValue(PropertyValue(), nameof(item), item);

    // derive
    public string Derived(string item) =>
        FormatValue(PropertyValue(), nameof(item), item);

    // add
    public string Add => PropertyValue();
    public string Added(string item) =>
        FormatValue(PropertyValue(), nameof(item), item);
    public string AddTitle(string item) =>
        FormatValue(PropertyValue(), nameof(item), item);
    public string AddHelp(string item) =>
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
    public MarkupString DeleteMarkupQuery(string item) =>
        (MarkupString)FormatValue(PropertyValue(), nameof(item), item);
    public string DeleteQuery(string item) =>
        FormatValue(PropertyValue(), nameof(item), item);
    public string DeleteTitle(string item) =>
        FormatValue(PropertyValue(), nameof(item), item);
    public string DeleteHelp(string item) =>
        FormatValue(PropertyValue(), nameof(item), item);
    public string UndoHelp(string item) =>
        FormatValue(PropertyValue(), nameof(item), item);

    // remove
    public string Remove => PropertyValue();
    public string RemoveQuery(string item) =>
        FormatValue(PropertyValue(), nameof(item), item);
    public string RemoveTitle(string item) =>
        FormatValue(PropertyValue(), nameof(item), item);
    public string RemoveHelp(string item) =>
        FormatValue(PropertyValue(), nameof(item), item);
    public string Removed(string item) =>
        FormatValue(PropertyValue(), nameof(item), item);
    public string RemoveAll => PropertyValue();
    public object SelectToChange => PropertyValue();

    // save
    public string SaveHelp(string item) =>
        FormatValue(PropertyValue(), nameof(item), item);
    public string Saved(string item) =>
        FormatValue(PropertyValue(), nameof(item), item);
    public string DeriveHelp(string item) =>
        FormatValue(PropertyValue(), nameof(item), item);
}