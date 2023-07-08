using Microsoft.Extensions.Localization;

namespace PayrollEngine.WebApp.Shared;

public class DialogLocalizer : LocalizerBase
{
    public DialogLocalizer(IStringLocalizerFactory factory) :
        base(factory)
    {
    }

    public string Ok => PropertyValue();
    public string Cancel => PropertyValue();
    public string Delete => PropertyValue();
    public string Save => PropertyValue();
    public string Close => PropertyValue();
}