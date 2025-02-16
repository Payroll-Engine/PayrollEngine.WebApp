using Microsoft.Extensions.Localization;
using System.Globalization;

namespace PayrollEngine.WebApp.Shared;

public class InputLocalizer(IStringLocalizerFactory factory, CultureInfo culture) : 
    LocalizerBase(factory, culture: culture)
{
    public string Clear => PropertyValue();
    public string Decrement => PropertyValue();
    public string Increment => PropertyValue();
}