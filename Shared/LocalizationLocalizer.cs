using Microsoft.Extensions.Localization;

namespace PayrollEngine.WebApp.Shared;

public class LocalizationLocalizer : LocalizerBase
{
    public LocalizationLocalizer(IStringLocalizerFactory factory) :
        base(factory)
    {
    }

    public string Localization => PropertyValue();
    public string Localizations => PropertyValue();
    public string NotAvailable => PropertyValue();

    public string LocalizationBase => PropertyValue();

    public string DialogTitle(string localization) =>
        FormatValue(PropertyValue(), nameof(localization), localization);
}