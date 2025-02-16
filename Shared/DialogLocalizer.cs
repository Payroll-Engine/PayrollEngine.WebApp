using Microsoft.Extensions.Localization;
using System.Globalization;

namespace PayrollEngine.WebApp.Shared;

public class DialogLocalizer(IStringLocalizerFactory factory, CultureInfo culture) : 
    LocalizerBase(factory, culture: culture)
{
    public string Ok => PropertyValue();
    public string Cancel => PropertyValue();
    public string Delete => PropertyValue();
    public string Save => PropertyValue();
    public string Close => PropertyValue();
}