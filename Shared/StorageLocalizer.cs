using Microsoft.Extensions.Localization;
using System.Globalization;

namespace PayrollEngine.WebApp.Shared;

public class StorageLocalizer(IStringLocalizerFactory factory, CultureInfo culture) : 
    LocalizerBase(factory, culture: culture)
{
    public string Storage => PropertyValue();
    public string StorageItem => PropertyValue();
    public string NotAvailable => PropertyValue();

    public string ClearStorage => PropertyValue();
    public string Cleared => PropertyValue();

    public string ClearQuery(int count) =>
        FormatValue(PropertyValue(), nameof(count), count);
}