using Microsoft.Extensions.Localization;

namespace PayrollEngine.WebApp.Shared;

public class PayrollLocalizer : LocalizerBase
{
    public PayrollLocalizer(IStringLocalizerFactory factory) :
        base(factory)
    {
    }

    public string Payroll => PropertyValue();
    public string Payrolls => PropertyValue();
    public string NotAvailable => PropertyValue();

    public string WageTypePeriod => PropertyValue();
    public string WageTypeRetro => PropertyValue();
    public string CollectorRetro => PropertyValue();
}