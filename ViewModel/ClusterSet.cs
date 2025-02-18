using System.Linq;

namespace PayrollEngine.WebApp.ViewModel;

/// <summary>
/// View model cluster set
/// </summary>
public class ClusterSet : Client.Model.ClusterSet
{
    /// <summary>
    /// Default constructor
    /// </summary>
    public ClusterSet()
    {
    }

    /// <summary>
    /// Copy constructor
    /// </summary>
    /// <param name="copySource">Copy source</param>
    public ClusterSet(ClusterSet copySource) :
        base(copySource)
    {
    }

    /// <summary>
    /// Base model constructor
    /// </summary>
    /// <param name="copySource">Copy source</param>
    public ClusterSet(Client.Model.ClusterSet copySource) :
        base(copySource)
    {
    }

    /// <summary>
    /// Include clusters as string
    /// </summary>
    public string IncludeClustersAsString
    {
        get => IncludeClusters != null ? string.Join(',', IncludeClusters) : null;
        set => IncludeClusters = value?.Split(',').ToList();
    }

    /// <summary>
    /// Exclude clusters as string
    /// </summary>
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
