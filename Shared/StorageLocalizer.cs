using Microsoft.Extensions.Localization;

namespace PayrollEngine.WebApp.Shared;

public class StorageLocalizer : LocalizerBase
{
    public StorageLocalizer(IStringLocalizerFactory factory) :
        base(factory)
    {
    }

    public string Storage => FromCaller();
    public string StorageItem => FromCaller();
    public string NotAvailable => FromCaller();

    public string ClearStorage => FromCaller();
    public string Cleared => FromCaller();

    public string ClearQuery(int count) =>
        string.Format(FromCaller(), count);
}