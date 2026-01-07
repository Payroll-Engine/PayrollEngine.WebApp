using Microsoft.Extensions.Localization;
using System.Globalization;

namespace PayrollEngine.WebApp.Shared;

public class RegulationLocalizer(IStringLocalizerFactory factory, CultureInfo culture) : 
    LocalizerBase(factory, culture: culture)
{
    public string Regulation => PropertyValue();
    public string Regulations => PropertyValue();
    public string NotAvailable => PropertyValue();

    public string Missing => PropertyValue();
    public string ValidFrom => PropertyValue();
    public string Namespace => PropertyValue();
    public string Version => PropertyValue();
    public string SharedRegulation => PropertyValue();

    public string InheritanceNew => PropertyValue();
    public string InheritanceDerived => PropertyValue();
    public string InheritanceBase => PropertyValue();
}