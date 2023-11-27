using Microsoft.Extensions.Localization;

namespace PayrollEngine.WebApp.Shared;

public class LookupValueLocalizer(IStringLocalizerFactory factory) : LocalizerBase(factory)
{
    public string LookupValue => PropertyValue();
    public string LookupValues => PropertyValue();

    public string RangeValue => PropertyValue();
}