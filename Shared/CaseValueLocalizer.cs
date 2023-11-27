using Microsoft.Extensions.Localization;

namespace PayrollEngine.WebApp.Shared;

public class CaseValueLocalizer(IStringLocalizerFactory factory) : LocalizerBase(factory)
{
    public string CaseValue => PropertyValue();
    public string CaseValues => PropertyValue();
    public string NotAvailable => PropertyValue();
}