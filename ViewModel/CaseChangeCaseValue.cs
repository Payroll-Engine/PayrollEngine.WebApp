using System;
using System.Globalization;
using System.Linq;

namespace PayrollEngine.WebApp.ViewModel;

/// <summary>
/// Case change case value
/// </summary>
public class CaseChangeCaseValue : Client.Model.CaseChangeCaseValue, IViewModel,
    IViewAttributeObject, IEquatable<CaseChangeCaseValue>
{
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
    /// The case value tags as compact string
    /// </summary>
    public string TagsAsText => Tags != null && Tags.Any() ?
        string.Join(",", Tags) : null;

    /// <summary>
    /// Format the case value to string
    /// </summary>
    /// <returns>The case value string representation</returns>
    public string ValueAsString(IValueFormatter valueFormatter, CultureInfo culture) =>
        valueFormatter.ToString(Value, ValueType, culture);

    /// <inheritdoc />
    public bool Equals(CaseChangeCaseValue compare) =>
        base.Equals(compare);

    /// <inheritdoc />
    public bool Equals(IViewModel compare) => Equals(compare as CaseChangeCaseValue);
}