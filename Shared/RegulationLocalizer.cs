using Microsoft.Extensions.Localization;

namespace PayrollEngine.WebApp.Shared;

public class RegulationLocalizer : LocalizerBase
{
    public RegulationLocalizer(IStringLocalizerFactory factory) :
        base(factory)
    {
    }

    public string Regulation => PropertyValue();
    public string Regulations => PropertyValue();
    public string NotAvailable => PropertyValue();

    public string Missing => PropertyValue();
    public string ValidFrom => PropertyValue();
    public string Version => PropertyValue();
    public string SharedRegulation => PropertyValue();

    public string InheritanceNew => PropertyValue();
    public string InheritanceDerived => PropertyValue();
    public string InheritanceBase => PropertyValue();
}