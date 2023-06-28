using Microsoft.Extensions.Localization;

namespace PayrollEngine.WebApp.Shared;

public class ClusterSetLocalizer : LocalizerBase
{
    public ClusterSetLocalizer(IStringLocalizerFactory factory) :
        base(factory)
    {
    }

    public string ClusterSet => FromCaller();
    public string ClusterSets => FromCaller();
    public string NotAvailable => FromCaller();

    public string IncludeClusters => FromCaller();
    public string ExcludeClusters => FromCaller();
}