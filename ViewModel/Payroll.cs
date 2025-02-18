using PayrollEngine.Client.Model;

namespace PayrollEngine.WebApp.ViewModel;

/// <summary>
/// View model payroll
/// </summary>
public class Payroll : Client.Model.Payroll, IKeyEquatable<Payroll>
{
    /// <summary>
    /// Default constructor
    /// </summary>
    public Payroll()
    {
    }

    /// <summary>
    /// Copy constructor
    /// </summary>
    /// <param name="copySource">Copy source</param>
    public Payroll(Payroll copySource) :
        base(copySource)
    {
    }

    /// <summary>
    /// Base model constructor
    /// </summary>
    /// <param name="copySource">Copy source</param>
    public Payroll(Client.Model.Payroll copySource) :
        base(copySource)
    {
    }

    /// <summary>Compare two objects</summary>
    /// <param name="compare">The object to compare with this</param>
    /// <returns>True for objects with the same data</returns>
    public bool Equals(Payroll compare) =>
        base.Equals(compare);

    /// <inheritdoc />
    public bool EqualKey(Payroll compare) =>
        base.EqualKey(compare);
}