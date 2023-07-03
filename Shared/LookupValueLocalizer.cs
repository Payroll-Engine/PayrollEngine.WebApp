using Microsoft.Extensions.Localization;

namespace PayrollEngine.WebApp.Shared;

public class LookupValueLocalizer : LocalizerBase
{
    public LookupValueLocalizer(IStringLocalizerFactory factory) :
        base(factory)
    {
    }

    public string LookupValue => PropertyValue();
    public string LookupValues => PropertyValue();

    public string RangeValue => PropertyValue();
}