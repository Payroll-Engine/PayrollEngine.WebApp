using Microsoft.Extensions.Localization;

namespace PayrollEngine.WebApp.Shared;

public class ReportLogLocalizer : LocalizerBase
{
    public ReportLogLocalizer(IStringLocalizerFactory factory) :
        base(factory)
    {
    }

    public string ReportLog => PropertyValue();
    public string ReportLogs => PropertyValue();
    public string NotAvailable => PropertyValue();

    public string Message => PropertyValue();
    public string ReportDate => PropertyValue();
}