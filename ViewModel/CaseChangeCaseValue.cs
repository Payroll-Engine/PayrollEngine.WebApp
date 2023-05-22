using System;
using System.Linq;

namespace PayrollEngine.WebApp.ViewModel;

public class CaseChangeCaseValue : Client.Model.CaseChangeCaseValue, IViewModel,
    IViewAttributeObject, IEquatable<CaseChangeCaseValue>
{
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
    /// The case value tags as compact string
    /// </summary>
    public string TagsAsText => Tags != null && Tags.Any() ?
        string.Join(",", Tags) : null;

    /// <summary>
    /// Format the case value to string
    /// </summary>
    /// <returns>The case value string representation</returns>
    public string ValueAsString(IValueFormatter valueFormatter)
    {
        string valueString;
        // culture support
        var culture = Attributes.GetCulture();
        if (ValueType.IsDecimal() && Attributes != null && !string.IsNullOrWhiteSpace(culture))
        {
            valueString = valueFormatter.ToString(Value, ValueType);
        }
        else
        {
            valueString = valueFormatter.ToString(Value, ValueType);
        }
        return valueString;
    }

    /// <inheritdoc />
    public bool Equals(CaseChangeCaseValue compare) =>
        base.Equals(compare);

    /// <inheritdoc />
    public bool Equals(IViewModel compare) => Equals(compare as CaseChangeCaseValue);
}