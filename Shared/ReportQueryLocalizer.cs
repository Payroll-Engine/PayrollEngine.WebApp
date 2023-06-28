using Microsoft.Extensions.Localization;

namespace PayrollEngine.WebApp.Shared;

public class ReportQueryLocalizer : LocalizerBase
{
    public ReportQueryLocalizer(IStringLocalizerFactory factory) :
        base(factory)
    {
    }

    public string ReportQuery => FromCaller();
    public string ReportQueries => FromCaller();
    public string NotAvailable => FromCaller();
}