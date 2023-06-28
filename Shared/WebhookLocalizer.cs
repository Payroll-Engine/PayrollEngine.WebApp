using Microsoft.Extensions.Localization;

namespace PayrollEngine.WebApp.Shared;

public class WebhookLocalizer : LocalizerBase
{
    public WebhookLocalizer(IStringLocalizerFactory factory) :
        base(factory)
    {
    }

    public string Webhook => FromCaller();
    public string Webhooks => FromCaller();
    public string NotAvailable => FromCaller();

    public string ReceiverAddress => FromCaller();
    public string Action => FromCaller();
}