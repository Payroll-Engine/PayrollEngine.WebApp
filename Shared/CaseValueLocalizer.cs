using Microsoft.Extensions.Localization;

namespace PayrollEngine.WebApp.Shared;

public class CaseValueLocalizer : LocalizerBase
{
    public CaseValueLocalizer(IStringLocalizerFactory factory) :
        base(factory)
    {
    }

    public string CaseValue => PropertyValue();
    public string CaseValues => PropertyValue();
    public string NotAvailable => PropertyValue();
}