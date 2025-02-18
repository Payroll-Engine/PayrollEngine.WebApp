using System;

namespace PayrollEngine.WebApp.ViewModel;

/// <summary>
/// View model payrun job
/// </summary>
public class PayrunJob : Client.Model.PayrunJob, IViewModel,
    IViewAttributeObject, IEquatable<PayrunJob>
{
    /// <summary>
    /// Default constructor
    /// </summary>
    public PayrunJob()
    {
    }

    /// <summary>
    /// Copy constructor
    /// </summary>
    /// <param name="copySource">Copy source</param>
    public PayrunJob(PayrunJob copySource) :
        base(copySource)
    {
        DivisionName = copySource.DivisionName;
    }

    /// <summary>
    /// Base model constructor
    /// </summary>
    /// <param name="copySource">Copy source</param>
    public PayrunJob(Client.Model.PayrunJob copySource) :
        base(copySource)
    {
    }

    /// <summary>The division name</summary>
    private string DivisionName { get; }

    #region Attributes

    /// <inheritdoc />
    public string GetStringAttribute(string name) =>
        Attributes?.GetStringAttributeValue(name);

    /// <inheritdoc />
    public decimal GetNumericAttribute(string name) =>
        Attributes?.GetDecimalAttributeValue(name) ?? 0;

    /// <inheritdoc />
    public bool GetBooleanAttribute(string name) =>
        Attributes?.GetBooleanAttributeValue(name) ?? false;

    #endregion

    /// <summary>Compare two objects</summary>
    /// <param name="compare">The object to compare with this</param>
    /// <returns>True for objects with the same data</returns>
    public bool Equals(PayrunJob compare) =>
        compare != null &&
        base.Equals(compare) &&
        string.Equals(DivisionName, compare.DivisionName);

    /// <summary>Compare two objects</summary>
    /// <param name="compare">The object to compare with this</param>
    /// <returns>True for objects with the same data</returns>
    public bool Equals(IViewModel compare) =>
        Equals(compare as PayrunJob);
}