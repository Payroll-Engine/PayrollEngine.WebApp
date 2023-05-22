using PayrollEngine.Client.Model;

namespace PayrollEngine.WebApp.ViewModel;

public class RegulationShare : Client.Model.RegulationShare, IKeyEquatable<RegulationShare>
{
    public RegulationShare()
    {
    }

    public RegulationShare(RegulationShare copySource) :
        base(copySource)
    {
    }

    public RegulationShare(Client.Model.RegulationShare copySource) :
        base(copySource)
    {
    }

    /// <summary>Compare two objects</summary>
    /// <param name="compare">The object to compare with this</param>
    /// <returns>True for objects with the same data</returns>
    public bool Equals(RegulationShare compare) =>
        base.Equals(compare);

    public bool EqualKey(RegulationShare compare) =>
        base.EqualKey(compare);
}