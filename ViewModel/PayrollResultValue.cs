using System;
using System.Globalization;
using System.Linq;

namespace PayrollEngine.WebApp.ViewModel;

/// <summary>
/// Payroll result value
/// </summary>
public class PayrollResultValue : Client.Model.PayrollResultValue, IEquatable<PayrollResultValue>,
    IViewAttributeObject
{
    public PayrollResultValue()
    {
    }

    public PayrollResultValue(PayrollResultValue copySource) :
        base(copySource)
    {
        CultureInfo = copySource.CultureInfo;
    }

    public PayrollResultValue(Client.Model.PayrollResultValue copySource) :
        base(copySource)
    {
    }

    /// <summary>
    /// Gets or sets the culture information
    /// </summary>
    /// <value>The culture information.</value>
    public CultureInfo CultureInfo { get; set; }

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

    /// <summary>
    /// The result tags as compact string
    /// </summary>
    public string ResultTagsAsText =>
        ResultTags != null && ResultTags.Any() ? string.Join(",", ResultTags) : null;

    /// <summary>
    /// The result attributes as compact string
    /// </summary>
    public string AttributesAsText =>
        Attributes.ToText();

    /// <summary>Compare two objects</summary>
    /// <param name="compare">The object to compare with this</param>
    /// <returns>True for objects with the same data</returns>
    public bool Equals(PayrollResultValue compare) =>
        compare != null &&
        base.Equals(compare) &&
        Equals(CultureInfo, compare.CultureInfo);
}