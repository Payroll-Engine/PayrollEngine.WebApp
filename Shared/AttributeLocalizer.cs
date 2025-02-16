using Microsoft.Extensions.Localization;
using System.Globalization;

namespace PayrollEngine.WebApp.Shared;

public class AttributeLocalizer(IStringLocalizerFactory factory, CultureInfo culture) : 
    LocalizerBase(factory, culture: culture)
{
    public string Attribute => PropertyValue();
    public string Attributes => PropertyValue();
    public string NotAvailable => PropertyValue();
}