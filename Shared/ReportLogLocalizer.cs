using Microsoft.Extensions.Localization;
using System.Globalization;

namespace PayrollEngine.WebApp.Shared;

public class ReportLogLocalizer(IStringLocalizerFactory factory, CultureInfo culture) : 
    LocalizerBase(factory, culture: culture)
{
    public string ReportLog => PropertyValue();
    public string ReportLogs => PropertyValue();
    public string NotAvailable => PropertyValue();

    public string Message => PropertyValue();
    public string ReportDate => PropertyValue();
}