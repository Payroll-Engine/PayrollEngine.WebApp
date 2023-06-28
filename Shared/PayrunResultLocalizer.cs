using Microsoft.Extensions.Localization;

namespace PayrollEngine.WebApp.Shared;

public class PayrunResultLocalizer : LocalizerBase
{
    public PayrunResultLocalizer(IStringLocalizerFactory factory) :
        base(factory)
    {
    }

    public string PayrunResult => FromCaller();
    public string PayrunResults => FromCaller();
    public string NotAvailable => FromCaller();

    public string Period => FromCaller();
    public string ResultKind => FromCaller();
    public string KindName => FromCaller();
    public string ResultValue => FromCaller();
}