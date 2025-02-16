using Microsoft.Extensions.Localization;
using System.Globalization;

namespace PayrollEngine.WebApp.Shared;

public class LookupLocalizer(IStringLocalizerFactory factory, CultureInfo culture) : 
    LocalizerBase(factory, culture: culture)
{
    public string Lookup => PropertyValue();
    public string Lookups => PropertyValue();

    public string LookupName => PropertyValue();
    public string ValueFieldName => PropertyValue();
    public string TextFieldName => PropertyValue();
    public string RangeSize => PropertyValue();
}