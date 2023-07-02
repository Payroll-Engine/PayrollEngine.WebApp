using Microsoft.Extensions.Localization;

namespace PayrollEngine.WebApp.Shared;

public class DialogLocalizer : LocalizerBase
{
    public DialogLocalizer(IStringLocalizerFactory factory) :
        base(factory)
    {
    }

    public string Ok => FromCaller();
    public string Cancel => FromCaller();
    public string Submit => FromCaller();
    public string Delete => FromCaller();
    public string Save => FromCaller();
    public string Close => FromCaller();
}