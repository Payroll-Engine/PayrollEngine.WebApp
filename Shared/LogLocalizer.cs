using Microsoft.Extensions.Localization;
using System.Globalization;

namespace PayrollEngine.WebApp.Shared;

public class LogLocalizer(IStringLocalizerFactory factory, CultureInfo culture) : 
    LocalizerBase(factory, culture: culture)
{
    public string Log => PropertyValue();
    public string Logs => PropertyValue();
    public string NotAvailable => PropertyValue();

    public string Level => PropertyValue();
    public string Message => PropertyValue();
    public string Comment => PropertyValue();
    public string OwnerType => PropertyValue();
}