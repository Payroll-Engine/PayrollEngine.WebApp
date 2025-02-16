using Microsoft.Extensions.Localization;
using System.Globalization;

namespace PayrollEngine.WebApp.Shared;

public class ReportParameterLocalizer(IStringLocalizerFactory factory, CultureInfo culture) : 
    LocalizerBase(factory, culture: culture)
{
    public string ReportParameter => PropertyValue();
    public string ReportParameters => PropertyValue();
    public string NotAvailable => PropertyValue();

    public string ParameterType => PropertyValue();
}