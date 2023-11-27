using Microsoft.Extensions.Localization;

namespace PayrollEngine.WebApp.Shared;

public class PayrunParameterLocalizer(IStringLocalizerFactory factory) : LocalizerBase(factory)
{
    public string PayrunParameter => PropertyValue();
    public string PayrunParameters => PropertyValue();
    public string NotAvailable => PropertyValue();

    public string NoParameters => PropertyValue();
    public string CountParameters(int count) =>
        FormatValue(PropertyValue(), nameof(count), count);
}