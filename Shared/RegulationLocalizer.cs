using Microsoft.Extensions.Localization;

namespace PayrollEngine.WebApp.Shared;

public class RegulationLocalizer : LocalizerBase
{
    public RegulationLocalizer(IStringLocalizerFactory factory) :
        base(factory)
    {
    }

    public string Regulation => FromCaller();
    public string Regulations => FromCaller();
    public string NotAvailable => FromCaller();

    public string Missing => FromCaller();
    public string ValidFrom => FromCaller();
    public string Version => FromCaller();
    public string SharedRegulation => FromCaller();

    public string InheritanceNew => FromCaller();
    public string InheritanceDerived => FromCaller();
    public string InheritanceBase => FromCaller();
}