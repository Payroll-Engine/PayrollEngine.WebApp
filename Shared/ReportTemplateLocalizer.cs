using Microsoft.Extensions.Localization;

namespace PayrollEngine.WebApp.Shared;

public class ReportTemplateLocalizer : LocalizerBase
{
    public ReportTemplateLocalizer(IStringLocalizerFactory factory) :
        base(factory)
    {
    }

    public string ReportTemplate => PropertyValue();
    public string ReportTemplates => PropertyValue();
    public string NotAvailable => PropertyValue();

    public string Content => PropertyValue();
    public string Schema => PropertyValue();
    public string Resource => PropertyValue();
}