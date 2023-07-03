using Microsoft.Extensions.Localization;

namespace PayrollEngine.WebApp.Shared;

public class WebhookLocalizer : LocalizerBase
{
    public WebhookLocalizer(IStringLocalizerFactory factory) :
        base(factory)
    {
    }

    public string Webhook => PropertyValue();
    public string Webhooks => PropertyValue();
    public string NotAvailable => PropertyValue();

    public string ReceiverAddress => PropertyValue();
    public string Action => PropertyValue();
}