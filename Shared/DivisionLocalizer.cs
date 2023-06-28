using Microsoft.Extensions.Localization;

namespace PayrollEngine.WebApp.Shared;

public class DivisionLocalizer : LocalizerBase
{
    public DivisionLocalizer(IStringLocalizerFactory factory) :
        base(factory)
    {
    }

    public string Division => FromCaller();
    public string Divisions => FromCaller();
    public string NotAvailable => FromCaller();
}