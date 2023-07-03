using Microsoft.Extensions.Localization;

namespace PayrollEngine.WebApp.Shared;

public class ClusterSetLocalizer : LocalizerBase
{
    public ClusterSetLocalizer(IStringLocalizerFactory factory) :
        base(factory)
    {
    }

    public string ClusterSet => PropertyValue();
    public string ClusterSets => PropertyValue();
    public string NotAvailable => PropertyValue();

    public string IncludeClusters => PropertyValue();
    public string ExcludeClusters => PropertyValue();
}