using PayrollEngine.Client.Model;

namespace PayrollEngine.WebApp.ViewModel;

/// <summary>
/// View model regulation
/// </summary>
public class Regulation : Client.Model.Regulation, IKeyEquatable<Regulation>
{
    /// <summary>
    /// Default constructor
    /// </summary>
    public Regulation()
    {
    }

    /// <summary>
    /// Copy constructor
    /// </summary>
    /// <param name="copySource">Copy source</param>
    public Regulation(Regulation copySource) :
        base(copySource)
    {
    }

    /// <summary>
    /// Base model constructor
    /// </summary>
    /// <param name="copySource">Copy source</param>
    public Regulation(Client.Model.Regulation copySource) :
        base(copySource)
    {
    }

    /// <summary>Compare two objects</summary>
    /// <param name="compare">The object to compare with this</param>
    /// <returns>True for objects with the same data</returns>
    public bool Equals(Regulation compare) =>
        base.Equals(compare);

    /// <inheritdoc />
    public bool EqualKey(Regulation compare) =>
        base.EqualKey(compare);
}