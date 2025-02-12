using Microsoft.Extensions.Localization;

namespace PayrollEngine.WebApp.Shared;

public class DatePickerLocalizer(IStringLocalizerFactory factory) : LocalizerBase(factory)
{
    public string PrevMonth => PropertyValue();
    public string NextMonth => PropertyValue();
    public string PrevYear => PropertyValue();
    public string NextYear => PropertyValue();
}