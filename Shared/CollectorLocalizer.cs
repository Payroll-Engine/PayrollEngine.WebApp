using Microsoft.Extensions.Localization;
using System.Globalization;

namespace PayrollEngine.WebApp.Shared;

public class CollectorLocalizer(IStringLocalizerFactory factory, CultureInfo culture) :
    LocalizerBase(factory, culture: culture)
{
    public string Collector => PropertyValue();
    public string Collectors => PropertyValue();

    public string CollectMode => PropertyValue();
    public string Negated => PropertyValue();
    public string Threshold => PropertyValue();
    public string StartExpression => PropertyValue();
    public string ApplyExpression => PropertyValue();
    public string EndExpression => PropertyValue();
    public string StartActions => PropertyValue();
    public string ApplyActions => PropertyValue();
    public string EndActions => PropertyValue();
    public string MinResult => PropertyValue();
    public string MaxResult => PropertyValue();
}