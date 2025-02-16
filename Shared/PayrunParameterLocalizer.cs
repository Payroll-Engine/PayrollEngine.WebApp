using Microsoft.Extensions.Localization;
using System.Globalization;

namespace PayrollEngine.WebApp.Shared;

public class PayrunParameterLocalizer(IStringLocalizerFactory factory, CultureInfo culture) : 
    LocalizerBase(factory, culture: culture)
{
    public string PayrunParameter => PropertyValue();
    public string PayrunParameters => PropertyValue();
    public string NotAvailable => PropertyValue();

    public string NoParameters => PropertyValue();
    public string CountParameters(int count) =>
        FormatValue(PropertyValue(), nameof(count), count);
}