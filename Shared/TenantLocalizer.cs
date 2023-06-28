using Microsoft.Extensions.Localization;

namespace PayrollEngine.WebApp.Shared;

public class TenantLocalizer : LocalizerBase
{
    public TenantLocalizer(IStringLocalizerFactory factory) :
        base(factory)
    {
    }

    public string Tenant => FromCaller();
    public string Tenants => FromCaller();
    public string NotAvailable => FromCaller();
}