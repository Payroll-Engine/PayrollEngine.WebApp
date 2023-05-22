using System.Linq;

namespace PayrollEngine.WebApp.ViewModel;

public class ClusterSet : Client.Model.ClusterSet
{
    public ClusterSet()
    {
    }

    public ClusterSet(ClusterSet copySource) :
        base(copySource)
    {
    }

    public ClusterSet(Client.Model.ClusterSet copySource) :
        base(copySource)
    {
    }

    public string IncludeClustersAsString
    {
        get => IncludeClusters != null ? string.Join(',', IncludeClusters) : null;
        set => IncludeClusters = value?.Split(',').ToList();
    }

    public string ExcludeClustersAsString
    {
        get => ExcludeClusters != null ? string.Join(',', ExcludeClusters) : null;
        set => ExcludeClusters = value?.Split(',').ToList();
    }

    /// <summary>Compare two objects</summary>
    /// <param name="compare">The object to compare with this</param>
    /// <returns>True for objects with the same data</returns>
    public bool Equals(ClusterSet compare) =>
        base.Equals(compare);
}
