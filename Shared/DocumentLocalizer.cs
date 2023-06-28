using Microsoft.Extensions.Localization;

namespace PayrollEngine.WebApp.Shared;

public class DocumentLocalizer : LocalizerBase
{
    public DocumentLocalizer(IStringLocalizerFactory factory) :
        base(factory)
    {
    }

    public string Document => FromCaller();
    public string Documents => FromCaller();
    public string NotAvailable => FromCaller();

    public string Attachments => FromCaller();
    public string ContentType => FromCaller();
    public string ContentUpload => FromCaller();
    public string Replace => FromCaller();
    public string ContentReplaceQuery => FromCaller();
    public string ClearContent=> FromCaller();

    public string DocumentRemoved => FromCaller();

    public string ClearContentQuery(int bytes) =>
        string.Format(FromCaller(), bytes);

    public string DownloadTitle(string name) =>
        string.Format(FromCaller(), name);
    public string ContentUploaded(string name) =>
        string.Format(FromCaller(), name);
    public string ContentUnchanged(string name) =>
        string.Format(FromCaller(), name);

    public string FileSize(int bytes) =>
        string.Format(FromCaller(), bytes);
    public string ItemDocuments(string itemName) =>
        string.Format(FromCaller(), itemName);
}