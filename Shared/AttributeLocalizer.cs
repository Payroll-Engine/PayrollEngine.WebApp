using Microsoft.Extensions.Localization;

namespace PayrollEngine.WebApp.Shared;

public class AttributeLocalizer : LocalizerBase
{
    public AttributeLocalizer(IStringLocalizerFactory factory) :
        base(factory)
    {
    }

    public string Attribute => FromCaller();
    public string Attributes => FromCaller();
    public string NotAvailable => FromCaller();
}