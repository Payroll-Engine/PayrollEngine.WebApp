using PayrollEngine.Client.Model;

namespace PayrollEngine.WebApp.ViewModel;

/// <summary>
/// View model regulation share
/// </summary>
public class RegulationShare : Client.Model.RegulationShare, IKeyEquatable<RegulationShare>
{
    /// <summary>
    /// Default constructor
    /// </summary>
    public RegulationShare()
    {
    }

    /// <summary>
    /// Copy constructor
    /// </summary>
    /// <param name="copySource">Copy source</param>
    public RegulationShare(RegulationShare copySource) :
        base(copySource)
    {
    }

    /// <summary>
    /// Base model constructor
    /// </summary>
    /// <param name="copySource">Copy source</param>
    public RegulationShare(Client.Model.RegulationShare copySource) :
        base(copySource)
    {
    }

    /// <summary>Compare two objects</summary>
    /// <param name="compare">The object to compare with this</param>
    /// <returns>True for objects with the same data</returns>
    public bool Equals(RegulationShare compare) =>
        base.Equals(compare);

    /// <inheritdoc />
    public bool EqualKey(RegulationShare compare) =>
        base.EqualKey(compare);
}