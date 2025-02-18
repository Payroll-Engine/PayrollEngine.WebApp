using System;
using System.Globalization;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using PayrollEngine.Client.Model;

namespace PayrollEngine.WebApp.ViewModel;

/// <summary>
/// View model payrun parameter
/// </summary>
public class PayrunParameter : Client.Model.PayrunParameter, IFieldObject
{
    /// <summary>
    /// Default constructor
    /// </summary>
    public PayrunParameter()
    {
    }

    /// <summary>
    /// Copy constructor
    /// </summary>
    /// <param name="copySource">Copy source</param>
    public PayrunParameter(PayrunParameter copySource) :
        base(copySource)
    {
    }

    #region Field Object and Variant Value

    /// <summary>
    /// Tenant culture
    /// </summary>
    [JsonIgnore]
    public CultureInfo TenantCulture { get; set; }

    /// <summary>
    /// Value formatter
    /// </summary>
    [JsonIgnore]
    public IValueFormatter ValueFormatter { get; set; }

    /// <summary>
    /// Lookup settings
    /// </summary>
    [JsonIgnore]
    LookupSettings IFieldObject.LookupSettings => null;

    /// <summary>
    /// No lookup values
    /// </summary>
    [JsonIgnore]
    List<LookupObject> IFieldObject.LookupValues => null;

    /// <inheritdoc />
    [JsonIgnore]
    bool IFieldObject.ValueMandatory => Mandatory;

    /// <inheritdoc />
    bool IFieldObject.IsValidValue() =>
        !Mandatory || (Mandatory && HasValue);

    /// <summary>
    /// Culture
    /// </summary>
    [JsonIgnore]
    public string Culture => null;

    /// <summary>
    /// Test for value
    /// </summary>
    [JsonIgnore]
    public bool HasValue => !string.IsNullOrWhiteSpace(Value);

    /// <inheritdoc />
    [JsonIgnore]
    public string ValueAsString
    {
        get => string.IsNullOrWhiteSpace(Value) ? null : ValueConvert.ToString(Value, TenantCulture);
        set => Value = ValueConvert.ToJson(value);
    }

    /// <inheritdoc />
    [JsonIgnore]
    public int? ValueAsInteger
    {
        get => string.IsNullOrWhiteSpace(Value) ? null : ValueConvert.ToInteger(Value, TenantCulture);
        set => Value = ValueConvert.ToJson(value);
    }

    /// <inheritdoc />
    [JsonIgnore]
    public bool? ValueAsBoolean
    {
        get => !string.IsNullOrWhiteSpace(Value) && ValueConvert.ToBoolean(Value, TenantCulture);
        set => Value = ValueConvert.ToJson(value);
    }

    /// <inheritdoc />
    [JsonIgnore]
    public decimal? ValueAsDecimal
    {
        get => string.IsNullOrWhiteSpace(Value) ? null : ValueConvert.ToDecimal(Value, TenantCulture);
        set => Value = ValueConvert.ToJson(value);

    }

    /// <inheritdoc />
    [JsonIgnore]
    public decimal? ValueAsPercent
    {
        get => ValueAsDecimal * 100;
        set => ValueAsDecimal = value / 100;
    }

    /// <inheritdoc />
    [JsonIgnore]
    public DateTime? ValueAsDateTime
    {
        get => Date.Parse(Value, TenantCulture) ??
               (string.IsNullOrWhiteSpace(Value) ? null : ValueConvert.ToDateTime(Value, TenantCulture));
        set => Value = ValueConvert.ToJson(value);
    }

    /// <summary>
    /// Get the localized name
    /// </summary>
    /// <param name="culture">Culture</param>
    public string GetLocalizedName(CultureInfo culture) =>
        culture.Name.GetLocalization(NameLocalizations, Name);

    /// <summary>
    /// Get localized description
    /// </summary>
    /// <param name="culture">Culture</param>
    public string GetLocalizedDescription(CultureInfo culture) =>
        culture.Name.GetLocalization(DescriptionLocalizations, Description);

    /// <summary>
    /// Format value
    /// </summary>
    /// <param name="culture">Culture</param>
    public string FormatValue(CultureInfo culture = null)
    {
        // priority 1: object culture
        if (!string.IsNullOrWhiteSpace(Culture))
        {
            culture = new CultureInfo(Culture);
        }
        // priority 2: parameter culture
        // priority 3: system culture
        culture ??= CultureInfo.CurrentCulture;

        return ValueFormatter.ToString(Value, ValueType, culture);
    }

    #endregion

    /// <inheritdoc />
    public override string ToString() =>
        $"{Name}={Value}";
}