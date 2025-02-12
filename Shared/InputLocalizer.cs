using Microsoft.Extensions.Localization;

namespace PayrollEngine.WebApp.Shared;

public class InputLocalizer(IStringLocalizerFactory factory) : LocalizerBase(factory)
{
    public string Clear => PropertyValue();
    public string Decrement => PropertyValue();
    public string Increment => PropertyValue();
}