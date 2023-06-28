using Microsoft.Extensions.Localization;

namespace PayrollEngine.WebApp.Shared;

public class LocalizationLocalizer : LocalizerBase
{
    public LocalizationLocalizer(IStringLocalizerFactory factory) :
        base(factory)
    {
    }

    public string Localization => FromCaller();
    public string Localizations => FromCaller();
    public string NotAvailable => FromCaller();

    public string LocalizationBase => FromCaller();

    public string DialogTitle(string item) =>
        string.Format(FromCaller(), item);
}