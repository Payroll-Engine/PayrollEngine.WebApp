using Microsoft.Extensions.Localization;

namespace PayrollEngine.WebApp.Shared;

public class NavGroupLocalizer(IStringLocalizerFactory factory) : LocalizerBase(factory)
{
    public string ToggleExpand => PropertyValue();
}