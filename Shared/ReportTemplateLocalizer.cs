using Microsoft.Extensions.Localization;
using System.Globalization;

namespace PayrollEngine.WebApp.Shared;

public class ReportTemplateLocalizer(IStringLocalizerFactory factory, CultureInfo culture) : 
    LocalizerBase(factory, culture: culture)
{
    public string ReportTemplate => PropertyValue();
    public string ReportTemplates => PropertyValue();
    public string NotAvailable => PropertyValue();

    public string Content => PropertyValue();
    public string ContentType => PropertyValue();
    public string Schema => PropertyValue();
    public string Resource => PropertyValue();
    public string MissingContentError => PropertyValue();
}