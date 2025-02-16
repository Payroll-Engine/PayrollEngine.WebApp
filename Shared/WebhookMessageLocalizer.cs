using Microsoft.Extensions.Localization;
using System.Globalization;

namespace PayrollEngine.WebApp.Shared;

public class WebhookMessageLocalizer(IStringLocalizerFactory factory, CultureInfo culture) : 
    LocalizerBase(factory, culture: culture)
{
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