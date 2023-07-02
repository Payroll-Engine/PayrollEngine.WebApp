using Microsoft.Extensions.Localization;

namespace PayrollEngine.WebApp.Shared;

public class LogLocalizer : LocalizerBase
{
    public LogLocalizer(IStringLocalizerFactory factory) :
        base(factory)
    {
    }

    public string Log => FromCaller();
    public string Logs => FromCaller();
    public string NotAvailable => FromCaller();

    public string Level => FromCaller();
    public string Message => FromCaller();
    public string Comment => FromCaller();
    public string OwnerType => FromCaller();
}