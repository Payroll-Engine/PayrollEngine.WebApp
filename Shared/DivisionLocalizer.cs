using Microsoft.Extensions.Localization;

namespace PayrollEngine.WebApp.Shared;

public class DivisionLocalizer(IStringLocalizerFactory factory) : LocalizerBase(factory)
{
    public string Division => PropertyValue();
    public string Divisions => PropertyValue();
    public string NotAvailable => PropertyValue();
    public string CultureHelp => PropertyValue();
    public string CalendarHelp => PropertyValue();
}