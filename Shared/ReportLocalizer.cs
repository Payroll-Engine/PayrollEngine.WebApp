using Microsoft.Extensions.Localization;

namespace PayrollEngine.WebApp.Shared;

public class ReportLocalizer : LocalizerBase
{
    public ReportLocalizer(IStringLocalizerFactory factory) :
        base(factory)
    {
    }

    public string Report => FromCaller();
    public string Reports => FromCaller();
    public string NotAvailable => FromCaller();

    public string Category => FromCaller();
    public string AttributeMode => FromCaller();
    public string Relation => FromCaller();
    public string RelationNotAvailable => FromCaller();
    public string Relations => FromCaller();
    public string BuildExpression => FromCaller();
    public string StartExpression => FromCaller();
    public string EndExpression => FromCaller();

    public string EmptyReport => FromCaller();

    public string PreparingDownload => FromCaller();
    public string ReportDownload => FromCaller();
    public string ExecutionError => FromCaller();

    public string Xml => FromCaller();
    public string XmlRaw => FromCaller();
    public string Excel => FromCaller();
    public string Word => FromCaller();
    public string Pdf => FromCaller();

    public string RelationParentTable => FromCaller();
    public string RelationParentColumn => FromCaller();
    public string RelationChildTable => FromCaller();
    public string RelationChildColumn => FromCaller();

    public string TemplateNotAvailable(string report, Language language) =>
        string.Format(FromCaller(), report, language);
    public string XmlValidationError(string report) =>
        string.Format(FromCaller(), report);
    public string EmptyXmlRaw(string report) =>
        string.Format(FromCaller(), report);
}