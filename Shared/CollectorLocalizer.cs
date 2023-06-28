using Microsoft.Extensions.Localization;

namespace PayrollEngine.WebApp.Shared;

public class CollectorLocalizer : LocalizerBase
{
    public CollectorLocalizer(IStringLocalizerFactory factory) :
        base(factory)
    {
    }

    public string Collector => FromCaller();
    public string Collectors => FromCaller();

    public string CollectType => FromCaller();
    public string Threshold => FromCaller();
    public string StartExpression => FromCaller();
    public string ApplyExpression => FromCaller();
    public string EndExpression => FromCaller();
    public string MinResult => FromCaller();
    public string MaxResult => FromCaller();
}