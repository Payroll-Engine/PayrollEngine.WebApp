using Microsoft.Extensions.Localization;

namespace PayrollEngine.WebApp.Shared;

public class ItemLocalizer : LocalizerBase
{
    public ItemLocalizer(IStringLocalizerFactory factory) :
        base(factory)
    {
    }

    public string CSharpExpression => FromCaller();
    public string BaseField => FromCaller();
    public string InitOnlyField => FromCaller();
    public string ReadOnlyField => FromCaller();

    // add/create
    public string Add => FromCaller();
    public string Added(string item) =>
        string.Format(FromCaller(), item);
    public string AddTitle(string item) =>
        string.Format(FromCaller(), item);
    public string AddHelp(string item) =>
        string.Format(FromCaller(), item);

    public string NotAvailable(string item) =>
        string.Format(FromCaller(), item);
    public string SelectParent(string item) =>
        string.Format(FromCaller(), item);

    // edit/updated
    public string Edit => FromCaller();
    public string EditTitle(string item) =>
        string.Format(FromCaller(), item);
    public string Updated(string item) =>
        string.Format(FromCaller(), item);
    public string EditHelp(string item) =>
        string.Format(FromCaller(), item);

    // delete
    public string Delete => FromCaller();
    public string Deleted(string item) =>
        string.Format(FromCaller(), item);
    public string DeleteQuery(string item) =>
        string.Format(FromCaller(), item);
    public string DeleteTitle(string item) =>
        string.Format(FromCaller(), item);
    public string DeleteHelp(string item) =>
        string.Format(FromCaller(), item);

    // remove
    public string Remove => FromCaller();
    public string RemoveTitle(string item) =>
        string.Format(FromCaller(), item);
    public string Removed(string item) =>
        string.Format(FromCaller(), item);
    public string RemoveAll => FromCaller();

    // save
    public string SaveHelp(string item) =>
        string.Format(FromCaller(), item);
    public string DeriveHelp(string item) =>
        string.Format(FromCaller(), item);
}