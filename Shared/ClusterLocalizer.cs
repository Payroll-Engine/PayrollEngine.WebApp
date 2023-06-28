using Microsoft.Extensions.Localization;

namespace PayrollEngine.WebApp.Shared;

public class ClusterLocalizer : LocalizerBase
{
    public ClusterLocalizer(IStringLocalizerFactory factory) :
        base(factory)
    {
    }

    public string Cluster => FromCaller();
    public string Clusters => FromCaller();
}