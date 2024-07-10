using Microsoft.Extensions.Localization;

namespace PayrollEngine.WebApp.Shared;

public class TenantLocalizer(IStringLocalizerFactory factory) : LocalizerBase(factory)
{
    public string Tenant => PropertyValue();
    public string Tenants => PropertyValue();
    public string NotAvailable => PropertyValue();
    public string CultureHelp => PropertyValue();
    public string CalendarHelp => PropertyValue();
}