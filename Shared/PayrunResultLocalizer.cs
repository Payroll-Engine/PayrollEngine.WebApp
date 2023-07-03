using Microsoft.Extensions.Localization;

namespace PayrollEngine.WebApp.Shared;

public class PayrunResultLocalizer : LocalizerBase
{
    public PayrunResultLocalizer(IStringLocalizerFactory factory) :
        base(factory)
    {
    }

    public string PayrunResult => PropertyValue();
    public string PayrunResults => PropertyValue();
    public string NotAvailable => PropertyValue();

    public string Period => PropertyValue();
    public string ResultKind => PropertyValue();
    public string KindName => PropertyValue();
}