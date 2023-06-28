using Microsoft.Extensions.Localization;

namespace PayrollEngine.WebApp.Shared;

public class LookupValueLocalizer : LocalizerBase
{
    public LookupValueLocalizer(IStringLocalizerFactory factory) :
        base(factory)
    {
    }

    public string LookupValue => FromCaller();
    public string LookupValues => FromCaller();

    public string RangeValue => FromCaller();
}