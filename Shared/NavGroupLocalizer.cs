using System.Globalization;
using Microsoft.Extensions.Localization;

namespace PayrollEngine.WebApp.Shared;

public class NavGroupLocalizer(IStringLocalizerFactory factory, CultureInfo culture) : 
    LocalizerBase(factory, culture: culture)
{
    public string ToggleExpand => PropertyValue();
}