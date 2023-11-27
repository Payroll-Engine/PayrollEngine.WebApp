using Microsoft.Extensions.Localization;

namespace PayrollEngine.WebApp.Shared;

public class ReportQueryLocalizer(IStringLocalizerFactory factory) : LocalizerBase(factory)
{
    public string ReportQuery => PropertyValue();
    public string ReportQueries => PropertyValue();
    public string NotAvailable => PropertyValue();
}