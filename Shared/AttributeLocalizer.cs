using Microsoft.Extensions.Localization;

namespace PayrollEngine.WebApp.Shared;

public class AttributeLocalizer : LocalizerBase
{
    public AttributeLocalizer(IStringLocalizerFactory factory) :
        base(factory)
    {
    }

    public string Attribute => PropertyValue();
    public string Attributes => PropertyValue();
    public string NotAvailable => PropertyValue();
}