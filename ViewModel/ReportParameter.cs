using System;
using System.Globalization;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using PayrollEngine.Client.Model;

namespace PayrollEngine.WebApp.ViewModel;

/// <summary>
/// View model report parameter
/// </summary>
public class ReportParameter : Client.Model.ReportParameter, IViewModel, IKeyEquatable<ReportParameter>, IFieldObject
{
    /// <summary>
    /// Default constructor
    /// </summary>
    // ReSharper disable once MemberCanBeProtected.Global
    public ReportParameter()
    {
    }

    /// <summary>
    /// Base model constructor
    /// </summary>
    /// <param name="copySource">Copy source</param>
    public ReportParameter(Client.Model.ReportParameter copySource) :
        base(copySource)
    {
    }

    #region Field Object and Variant Value

    /// <summary>
    /// Parent changed event
    /// </summary>
    public event Action<ReportParameter> ParameterChanged;

    public bool HasParameterChangedListener => ParameterChanged != null;

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

    /// <inheritdoc />
    [JsonIgnore]
    bool IFieldObject.ValueMandatory => Mandatory;

    /// <inheritdoc />
    bool IFieldObject.IsValidValue() => true;

    /// <inheritdoc />
    [JsonIgnore]
    public bool HasValue => !string.IsNullOrWhiteSpace(Value);

    /// <inheritdoc />
    [JsonIgnore]
    public string ValueAsString
    {
        get => string.IsNullOrWhiteSpace(Value) ? null : ValueConvert.ToString(Value, TenantCulture);
        set
        {
            if (!string.Equals(ValueAsString, value))
            {
                Value = value;
                OnReportParameterChanged();
            }
        }
    }

    /// <inheritdoc />
    [JsonIgnore]
    public int? ValueAsInteger
    {
        get => string.IsNullOrWhiteSpace(Value) ? null : ValueConvert.ToInteger(Value, TenantCulture);
        set
        {
            if (value != ValueAsInteger)
            {
                Value = ValueConvert.ToJson(value);
                OnReportParameterChanged();
            }
        }
    }

    /// <inheritdoc />
    [JsonIgnore]
    public bool? ValueAsBoolean
    {
        get => !string.IsNullOrWhiteSpace(Value) && ValueConvert.ToBoolean(Value, TenantCulture);
        set
        {
            if (value != ValueAsBoolean)
            {
                Value = ValueConvert.ToJson(value);
                OnReportParameterChanged();
            }
        }
    }

    /// <inheritdoc />
    [JsonIgnore]
    public decimal? ValueAsDecimal
    {
        get => string.IsNullOrWhiteSpace(Value) ? null : ValueConvert.ToDecimal(Value, TenantCulture);
        set
        {
            if (value != ValueAsDecimal)
            {
                Value = ValueConvert.ToJson(value);
                OnReportParameterChanged();
            }
        }
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
        set
        {
            if (value != ValueAsDateTime)
            {
                Value = ValueConvert.ToJson(value);
                OnReportParameterChanged();
            }
        }
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
    /// <param name="cultureInfo">Culture</param>
    public string FormatValue(CultureInfo cultureInfo = null)
    {
        // priority 1: parameter culture
        // priority 2: system culture
        cultureInfo ??= CultureInfo.CurrentCulture;

        return ValueFormatter.ToString(Value, ValueType, cultureInfo);
    }

    private void OnReportParameterChanged() =>
        ParameterChanged?.Invoke(this);

    #endregion

    /// <summary>Compare two objects</summary>
    /// <param name="compare">The object to compare with this</param>
    /// <returns>True for objects with the same data</returns>
    public bool Equals(ReportParameter compare) =>
        base.Equals(compare);

    /// <summary>Compare two objects</summary>
    /// <param name="compare">The object to compare with this</param>
    /// <returns>True for objects with the same data</returns>
    public bool Equals(IViewModel compare) =>
        Equals(compare as ReportParameter);

    /// <inheritdoc />
    public bool EqualKey(ReportParameter compare) =>
        base.EqualKey(compare);
}