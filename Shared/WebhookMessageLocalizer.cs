using Microsoft.Extensions.Localization;

namespace PayrollEngine.WebApp.Shared;

public class WebhookMessageLocalizer : LocalizerBase
{
    public WebhookMessageLocalizer(IStringLocalizerFactory factory) :
        base(factory)
    {
    }

    public string WebhookMessage => PropertyValue();
    public string WebhookMessages => PropertyValue();
    public string NotAvailable => PropertyValue();

    public string RequestDate => PropertyValue();
    public string RequestMessage => PropertyValue();
    public string RequestOperation => PropertyValue();
    public string ResponseDate => PropertyValue();
    public string ResponseStatus => PropertyValue();
    public string ResponseMessage => PropertyValue();
}