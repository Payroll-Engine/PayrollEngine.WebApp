using Microsoft.Extensions.Localization;
using System.Globalization;

namespace PayrollEngine.WebApp.Shared;

public class ReportLocalizer(IStringLocalizerFactory factory, CultureInfo culture) : 
    LocalizerBase(factory, culture: culture)
{
    public string Report => PropertyValue();
    public string Reports => PropertyValue();
    public string NotAvailable => PropertyValue();

    public string Category => PropertyValue();
    public string AttributeMode => PropertyValue();
    public string Relation => PropertyValue();
    public string RelationNotAvailable => PropertyValue();
    public string Relations => PropertyValue();
    public string BuildExpression => PropertyValue();
    public string StartExpression => PropertyValue();
    public string EndExpression => PropertyValue();

    public string EmptyReport => PropertyValue();
    public string BuildReport => PropertyValue();

    public string BuildingReport => PropertyValue();
    public string ExecutionError => PropertyValue();

    public string Json => PropertyValue();
    public string Xml => PropertyValue();
    public string Excel => PropertyValue();
    public string Word => PropertyValue();
    public string Pdf => PropertyValue();

    public string RelationParentTable => PropertyValue();
    public string RelationParentColumn => PropertyValue();
    public string RelationChildTable => PropertyValue();
    public string RelationChildColumn => PropertyValue();

    public string TemplateNotAvailable(string report, string culture) =>
        FormatValue(PropertyValue(), nameof(report), report, nameof(culture), culture);
    public string XmlValidationError(string report) =>
        FormatValue(PropertyValue(), nameof(report), report);
    public string EmptyXml(string report) =>
        FormatValue(PropertyValue(), nameof(report), report);
}