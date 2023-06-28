using Microsoft.Extensions.Localization;

namespace PayrollEngine.WebApp.Shared;

public class CaseValueLocalizer : LocalizerBase
{
    public CaseValueLocalizer(IStringLocalizerFactory factory) :
        base(factory)
    {
    }

    public string CaseValue => FromCaller();
    public string CaseValues => FromCaller();
    public string NotAvailable => FromCaller();
}