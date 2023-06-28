using Microsoft.Extensions.Localization;

namespace PayrollEngine.WebApp.Shared;

public class ReportTemplateLocalizer : LocalizerBase
{
    public ReportTemplateLocalizer(IStringLocalizerFactory factory) :
        base(factory)
    {
    }

    public string ReportTemplate => FromCaller();
    public string ReportTemplates => FromCaller();
    public string NotAvailable => FromCaller();

    public string Content => FromCaller();
    public string ContentType => FromCaller();
    public string Schema => FromCaller();
    public string Resource => FromCaller();
}