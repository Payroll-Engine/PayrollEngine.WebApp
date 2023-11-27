using Microsoft.Extensions.Localization;

namespace PayrollEngine.WebApp.Shared;

public class ReportParameterLocalizer(IStringLocalizerFactory factory) : LocalizerBase(factory)
{
    public string ReportParameter => PropertyValue();
    public string ReportParameters => PropertyValue();
    public string NotAvailable => PropertyValue();

    public string ParameterType => PropertyValue();
}