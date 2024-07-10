using Microsoft.Extensions.Localization;

namespace PayrollEngine.WebApp.Shared;

public class DocumentLocalizer(IStringLocalizerFactory factory) : LocalizerBase(factory)
{
    public string Document => PropertyValue();
    public string Documents => PropertyValue();
    public string NotAvailable => PropertyValue();

    public string Attachments => PropertyValue();
    public string ContentType => PropertyValue();
    public string ContentUpload => PropertyValue();
    public string Replace => PropertyValue();
    public string ContentReplaceQuery => PropertyValue();
    public string ClearContent=> PropertyValue();

    public string DocumentRemoved => PropertyValue();

    public string ClearContentQuery(int bytes) =>
        FormatValue(PropertyValue(), nameof(bytes), bytes);
    public string FileSize(int bytes) =>
        FormatValue(PropertyValue(), nameof(bytes), bytes);

    public string DownloadTitle(string document) =>
        FormatValue(PropertyValue(), nameof(document), document);
    public string ContentUploaded(string document) =>
        FormatValue(PropertyValue(), nameof(document), document);
    public string ContentUnchanged(string document) =>
        FormatValue(PropertyValue(), nameof(document), document);
}