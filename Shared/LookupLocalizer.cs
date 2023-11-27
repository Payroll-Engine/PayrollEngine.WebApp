using Microsoft.Extensions.Localization;

namespace PayrollEngine.WebApp.Shared;

public class LookupLocalizer(IStringLocalizerFactory factory) : LocalizerBase(factory)
{
    public string Lookup => PropertyValue();
    public string Lookups => PropertyValue();

    public string LookupName => PropertyValue();
    public string ValueFieldName => PropertyValue();
    public string TextFieldName => PropertyValue();
    public string RangeSize => PropertyValue();
}