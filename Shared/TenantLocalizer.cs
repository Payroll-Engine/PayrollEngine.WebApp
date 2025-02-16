using Microsoft.Extensions.Localization;
using System.Globalization;

namespace PayrollEngine.WebApp.Shared;

public class TenantLocalizer(IStringLocalizerFactory factory, CultureInfo culture) : 
    LocalizerBase(factory, culture: culture)
{
    public string Tenant => PropertyValue();
    public string Tenants => PropertyValue();
    public string NotAvailable => PropertyValue();
    public string CultureHelp => PropertyValue();
    public string CalendarHelp => PropertyValue();
    public string UserTenant(string tenant) =>
        FormatValue(PropertyValue(), nameof(tenant), tenant);
}