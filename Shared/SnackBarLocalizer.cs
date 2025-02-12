using Microsoft.Extensions.Localization;

namespace PayrollEngine.WebApp.Shared;

public class SnackBarLocalizer(IStringLocalizerFactory factory) : LocalizerBase(factory)
{
    public string Close => PropertyValue();
}