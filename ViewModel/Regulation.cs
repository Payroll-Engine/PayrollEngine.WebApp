using PayrollEngine.Client.Model;

namespace PayrollEngine.WebApp.ViewModel;

public class Regulation : Client.Model.Regulation, IKeyEquatable<Regulation>
{
    public Regulation()
    {
    }

    public Regulation(Regulation copySource) :
        base(copySource)
    {
    }

    public Regulation(Client.Model.Regulation copySource) :
        base(copySource)
    {
    }

    /// <summary>Compare two objects</summary>
    /// <param name="compare">The object to compare with this</param>
    /// <returns>True for objects with the same data</returns>
    public bool Equals(Regulation compare) =>
        base.Equals(compare);

    public bool EqualKey(Regulation compare) =>
        base.EqualKey(compare);
}