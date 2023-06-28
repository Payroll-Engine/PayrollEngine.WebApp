using Microsoft.Extensions.Localization;

namespace PayrollEngine.WebApp.Shared;

public class WebhookMessageLocalizer : LocalizerBase
{
    public WebhookMessageLocalizer(IStringLocalizerFactory factory) :
        base(factory)
    {
    }

    public string WebhookMessage => FromCaller();
    public string WebhookMessages => FromCaller();
    public string NotAvailable => FromCaller();

    public string RequestDate => FromCaller();
    public string RequestMessage => FromCaller();
    public string RequestOperation => FromCaller();
    public string ResponseDate => FromCaller();
    public string ResponseStatus => FromCaller();
    public string ResponseMessage => FromCaller();
}