using Microsoft.Extensions.Localization;
using System.Globalization;

namespace PayrollEngine.WebApp.Shared;

public class LookupValueLocalizer(IStringLocalizerFactory factory, CultureInfo culture) : 
    LocalizerBase(factory, culture: culture)
{
    public string LookupValue => PropertyValue();
    public string LookupValues => PropertyValue();

    public string RangeValue => PropertyValue();
}