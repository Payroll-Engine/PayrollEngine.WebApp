using PayrollEngine.Client.Model;

namespace PayrollEngine.WebApp.ViewModel;

/// <summary>
/// View model payroll layer
/// </summary>
public class PayrollLayer : Client.Model.PayrollLayer, IKeyEquatable<PayrollLayer>
{
    /// <summary>
    /// Default constructor
    /// </summary>
    public PayrollLayer()
    {
    }

    /// <summary>
    /// Copy constructor
    /// </summary>
    /// <param name="copySource">Copy source</param>
    public PayrollLayer(PayrollLayer copySource) :
        base(copySource)
    {
    }

    /// <summary>
    /// Base model constructor
    /// </summary>
    /// <param name="copySource">Copy source</param>
    public PayrollLayer(Client.Model.PayrollLayer copySource) :
        base(copySource)
    {
    }

    /// <summary>
    /// Compare order by level and priority
    /// </summary>
    /// <param name="compare">The layer to compare</param>
    public bool IsLowerLayerOrder(PayrollLayer compare) =>
        Level < compare.Level ||
        Level == compare.Level && Priority < compare.Priority;

    /// <summary>Compare two objects</summary>
    /// <param name="compare">The object to compare with this</param>
    /// <returns>True for objects with the same data</returns>
    public bool Equals(PayrollLayer compare) =>
        base.Equals(compare);

    /// <inheritdoc />
    public bool EqualKey(PayrollLayer compare) =>
        base.EqualKey(compare);
}