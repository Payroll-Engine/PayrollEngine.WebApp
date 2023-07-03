using Microsoft.Extensions.Localization;

namespace PayrollEngine.WebApp.Shared;

public class WageTypeLocalizer : LocalizerBase
{
    public WageTypeLocalizer(IStringLocalizerFactory factory) :
        base(factory)
    {
    }

    public string WageType => PropertyValue();
    public string WageTypes => PropertyValue();
    public string WageTypeNumber => PropertyValue();
    public string CollectorGroups => PropertyValue();
    public string ValueExpression => PropertyValue();
    public string ResultExpression => PropertyValue();
    public string CalendarHelp => PropertyValue();
}