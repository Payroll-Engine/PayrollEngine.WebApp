using Microsoft.Extensions.Localization;
using System.Globalization;

namespace PayrollEngine.WebApp.Shared;

public class WebhookLocalizer(IStringLocalizerFactory factory, CultureInfo culture) : 
    LocalizerBase(factory, culture: culture)
{
    public string Webhook => PropertyValue();
    public string Webhooks => PropertyValue();
    public string NotAvailable => PropertyValue();

    public string ReceiverAddress => PropertyValue();
    public string Action => PropertyValue();
}