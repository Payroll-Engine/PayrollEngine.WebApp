using Microsoft.Extensions.Localization;

namespace PayrollEngine.WebApp.Shared;

public class WageTypeLocalizer : LocalizerBase
{
    public WageTypeLocalizer(IStringLocalizerFactory factory) :
        base(factory)
    {
    }

    public string WageType => FromCaller();
    public string WageTypes => FromCaller();

    public string WageTypeNumber => FromCaller();
    public string CollectorGroups => FromCaller();
    public string ValueExpression => FromCaller();
    public string ResultExpression => FromCaller();
}