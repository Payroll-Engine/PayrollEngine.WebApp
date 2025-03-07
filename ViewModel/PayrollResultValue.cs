﻿using System;
using System.Globalization;
using System.Linq;

namespace PayrollEngine.WebApp.ViewModel;

/// <summary>
/// Payroll result value
/// </summary>
public class PayrollResultValue : Client.Model.PayrollResultValue, IEquatable<PayrollResultValue>,
    IViewAttributeObject
{
    /// <summary>
    /// Default constructor
    /// </summary>
    public PayrollResultValue()
    {
    }

    /// <summary>
    /// Copy constructor
    /// </summary>
    /// <param name="copySource">Copy source</param>
    public PayrollResultValue(PayrollResultValue copySource) :
        base(copySource)
    {
        CultureInfo = copySource.CultureInfo;
    }

    /// <summary>
    /// Base model constructor
    /// </summary>
    /// <param name="copySource">Copy source</param>
    public PayrollResultValue(Client.Model.PayrollResultValue copySource) :
        base(copySource)
    {
    }

    /// <summary>
    /// Gets or sets the culture information
    /// </summary>
    /// <value>The culture information.</value>
    private CultureInfo CultureInfo { get; }

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