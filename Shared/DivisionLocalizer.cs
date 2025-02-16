using Microsoft.Extensions.Localization;
using System.Globalization;

namespace PayrollEngine.WebApp.Shared;

public class DivisionLocalizer(IStringLocalizerFactory factory, CultureInfo culture) : 
    LocalizerBase(factory, culture: culture)
{
    public string Division => PropertyValue();
    public string Divisions => PropertyValue();
    public string NotAvailable => PropertyValue();
    public string CultureHelp => PropertyValue();
    public string CalendarHelp => PropertyValue();
}