using Microsoft.Extensions.Localization;

namespace PayrollEngine.WebApp.Shared;

public class ReportLogLocalizer : LocalizerBase
{
    public ReportLogLocalizer(IStringLocalizerFactory factory) :
        base(factory)
    {
    }

    public string ReportLog => FromCaller();
    public string ReportLogs => FromCaller();
    public string NotAvailable => FromCaller();

    public string Message => FromCaller();
    public string ReportDate => FromCaller();
}