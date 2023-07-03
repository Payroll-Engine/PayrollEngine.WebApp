using Microsoft.Extensions.Localization;

namespace PayrollEngine.WebApp.Shared;

public class ReportQueryLocalizer : LocalizerBase
{
    public ReportQueryLocalizer(IStringLocalizerFactory factory) :
        base(factory)
    {
    }

    public string ReportQuery => PropertyValue();
    public string ReportQueries => PropertyValue();
    public string NotAvailable => PropertyValue();
}