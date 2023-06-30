using Microsoft.Extensions.Localization;

namespace PayrollEngine.WebApp.Shared;

public class PayrollLocalizer : LocalizerBase
{
    public PayrollLocalizer(IStringLocalizerFactory factory) :
        base(factory)
    {
    }

    public string Payroll => FromCaller();
    public string Payrolls => FromCaller();
    public string NotAvailable => FromCaller();

    public string WageTypePeriod => FromCaller();
    public string WageTypeRetro => FromCaller();
    public string CollectorRetro => FromCaller();
    public string ClusterSets => FromCaller();
}