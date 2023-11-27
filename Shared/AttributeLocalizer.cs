using Microsoft.Extensions.Localization;

namespace PayrollEngine.WebApp.Shared;

public class AttributeLocalizer(IStringLocalizerFactory factory) : LocalizerBase(factory)
{
    public string Attribute => PropertyValue();
    public string Attributes => PropertyValue();
    public string NotAvailable => PropertyValue();
}