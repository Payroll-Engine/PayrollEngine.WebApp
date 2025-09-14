using Microsoft.Extensions.Localization;
using System.Globalization;

namespace PayrollEngine.WebApp.Shared;

public class TimePickerLocalizer(IStringLocalizerFactory factory, CultureInfo culture) : 
    LocalizerBase(factory, culture: culture)
{
    public string Open => PropertyValue();
    public string PrevMonth => PropertyValue();
    public string NextMonth => PropertyValue();
    public string PrevYear => PropertyValue();
    public string NextYear => PropertyValue();
}