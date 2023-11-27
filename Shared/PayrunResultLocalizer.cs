using Microsoft.Extensions.Localization;

namespace PayrollEngine.WebApp.Shared;

public class PayrunResultLocalizer(IStringLocalizerFactory factory) : LocalizerBase(factory)
{
    public string PayrunResult => PropertyValue();
    public string PayrunResults => PropertyValue();
    public string NotAvailable => PropertyValue();

    public string Period => PropertyValue();
    public string ResultKind => PropertyValue();
    public string KindName => PropertyValue();
}