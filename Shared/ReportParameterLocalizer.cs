using Microsoft.Extensions.Localization;

namespace PayrollEngine.WebApp.Shared;

public class ReportParameterLocalizer : LocalizerBase
{
    public ReportParameterLocalizer(IStringLocalizerFactory factory) :
        base(factory)
    {
    }

    public string ReportParameter => FromCaller();
    public string ReportParameters => FromCaller();
    public string NotAvailable => FromCaller();

    public string ParameterType => FromCaller();
}