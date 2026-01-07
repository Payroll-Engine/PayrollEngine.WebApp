using Microsoft.Extensions.Localization;
using System.Globalization;

namespace PayrollEngine.WebApp.Shared;

public class WageTypeLocalizer(IStringLocalizerFactory factory, CultureInfo culture) :
    LocalizerBase(factory, culture: culture)
{
    public string WageType => PropertyValue();
    public string WageTypes => PropertyValue();
    public string WageTypeNumber => PropertyValue();
    public string CollectorGroups => PropertyValue();
    public string ValueExpression => PropertyValue();
    public string ResultExpression => PropertyValue();
    public string ValueActions => PropertyValue();
    public string ResultActions => PropertyValue();
    public string CalendarHelp => PropertyValue();
}