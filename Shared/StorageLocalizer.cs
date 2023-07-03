using Microsoft.Extensions.Localization;

namespace PayrollEngine.WebApp.Shared;

public class StorageLocalizer : LocalizerBase
{
    public StorageLocalizer(IStringLocalizerFactory factory) :
        base(factory)
    {
    }

    public string Storage => PropertyValue();
    public string StorageItem => PropertyValue();
    public string NotAvailable => PropertyValue();

    public string ClearStorage => PropertyValue();
    public string Cleared => PropertyValue();

    public string ClearQuery(int count) =>
        FormatValue(PropertyValue(), nameof(count), count);
}