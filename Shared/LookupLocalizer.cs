using Microsoft.Extensions.Localization;

namespace PayrollEngine.WebApp.Shared;

public class LookupLocalizer : LocalizerBase
{
    public LookupLocalizer(IStringLocalizerFactory factory) :
        base(factory)
    {
    }

    public string Lookup => FromCaller();
    public string Lookups => FromCaller();

    public string LookupName => FromCaller();
    public string ValueFieldName => FromCaller();
    public string TextFieldName => FromCaller();
    public string RangeSize => FromCaller();
}