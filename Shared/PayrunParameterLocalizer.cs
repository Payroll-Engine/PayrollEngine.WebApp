using Microsoft.Extensions.Localization;

namespace PayrollEngine.WebApp.Shared;

public class PayrunParameterLocalizer : LocalizerBase
{
    public PayrunParameterLocalizer(IStringLocalizerFactory factory) :
        base(factory)
    {
    }

    public string PayrunParameter => FromCaller();
    public string PayrunParameters => FromCaller();
    public string NotAvailable => FromCaller();

    public string NoParameters => FromCaller();
    public string CountParameters(int count) =>
        string.Format(FromCaller(), count);
}