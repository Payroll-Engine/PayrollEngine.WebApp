using System;

namespace PayrollEngine.WebApp.ViewModel;

public class PayrunJob : Client.Model.PayrunJob, IViewModel,
    IViewAttributeObject, IEquatable<PayrunJob>
{
    public PayrunJob()
    {
    }

    public PayrunJob(PayrunJob copySource) :
        base(copySource)
    {
        DivisionName = copySource.DivisionName;
    }

    public PayrunJob(Client.Model.PayrunJob copySource) :
        base(copySource)
    {
    }

    /// <summary>The division name</summary>
    public string DivisionName { get; set; }

    #region Attributes

    /// <inheritdoc />
    public string GetStringAttribute(string name) =>
        Attributes?.GetStringAttributeValue(name);

    /// <inheritdoc />
    public decimal GetNumericAttribute(string name) =>
        Attributes?.GetDecimalAttributeValue(name) ?? default;

    /// <inheritdoc />
    public bool GetBooleanAttribute(string name) =>
        Attributes?.GetBooleanAttributeValue(name) ?? default;

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