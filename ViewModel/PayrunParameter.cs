using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.Json.Serialization;
using PayrollEngine.Client.Model;

namespace PayrollEngine.WebApp.ViewModel;

public class PayrunParameter : Client.Model.PayrunParameter, IFieldObject
{

    public PayrunParameter()
    {
    }

    public PayrunParameter(PayrunParameter copySource) :
        base(copySource)
    {
    }

    #region Field Object and Variant Value

    [JsonIgnore]
    public CultureInfo TenantCulture { get; set; }

    [JsonIgnore]
    public IValueFormatter ValueFormatter { get; set; }

    [JsonIgnore]
    LookupSettings IFieldObject.LookupSettings => null;

    /// <summary>
    /// No lookup values
    /// </summary>
    [JsonIgnore]
    List<LookupObject> IFieldObject.LookupValues => null;

    [JsonIgnore]
    bool IFieldObject.ValueMandatory => Mandatory;

    bool IFieldObject.IsValidValue() =>
        !Mandatory || (Mandatory && HasValue);

    [JsonIgnore]
    public string Culture => null;

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

    public string GetLocalizedName(CultureInfo culture) =>
        culture.Name.GetLocalization(NameLocalizations, Name);

    public string GetLocalizedDescription(CultureInfo culture) =>
        culture.Name.GetLocalization(DescriptionLocalizations, Description);

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

    public override string ToString() =>
        $"{Name}={Value}";
}