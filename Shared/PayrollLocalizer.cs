using Microsoft.Extensions.Localization;
using System.Globalization;

namespace PayrollEngine.WebApp.Shared;

public class PayrollLocalizer(IStringLocalizerFactory factory, CultureInfo culture) : 
    LocalizerBase(factory, culture: culture)
{
    public string Payroll => PropertyValue();
    public string Payrolls => PropertyValue();
    public string NotAvailable => PropertyValue();

    public string WageTypePeriod => PropertyValue();
    public string WageTypeRetro => PropertyValue();
    public string CollectorRetro => PropertyValue();
}