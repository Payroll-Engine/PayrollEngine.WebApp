using Microsoft.Extensions.Localization;
using System.Globalization;

namespace PayrollEngine.WebApp.Shared;

public class LocalizationLocalizer(IStringLocalizerFactory factory, CultureInfo culture) : 
    LocalizerBase(factory, culture: culture)
{
    public string Localization => PropertyValue();
    public string Localizations => PropertyValue();
    public string NotAvailable => PropertyValue();

    public string LocalizationBase => PropertyValue();

    public string DialogTitle(string localization) =>
        FormatValue(PropertyValue(), nameof(localization), localization);
}