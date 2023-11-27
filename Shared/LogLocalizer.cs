using Microsoft.Extensions.Localization;

namespace PayrollEngine.WebApp.Shared;

public class LogLocalizer(IStringLocalizerFactory factory) : LocalizerBase(factory)
{
    public string Log => PropertyValue();
    public string Logs => PropertyValue();
    public string NotAvailable => PropertyValue();

    public string Level => PropertyValue();
    public string Message => PropertyValue();
    public string Comment => PropertyValue();
    public string OwnerType => PropertyValue();
}