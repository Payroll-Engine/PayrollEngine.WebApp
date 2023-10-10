using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.Json.Serialization;
using PayrollEngine.Client.Model;

namespace PayrollEngine.WebApp.ViewModel;

public class ReportParameter : Client.Model.ReportParameter, IViewModel, IKeyEquatable<ReportParameter>, IFieldObject
{
    // ReSharper disable once MemberCanBeProtected.Global
    public ReportParameter()
    {
    }

    protected ReportParameter(ReportParameter copySource) :
        base(copySource)
    {
    }

    public ReportParameter(Client.Model.ReportParameter copySource) :
        base(copySource)
    {
    }

    #region Field Object and Variant Value

    public event Action<ReportParameter> ParameterChanged;

    [JsonIgnore]
    LookupSettings IFieldObject.LookupSettings => null;

    /// <summary>
    /// No lookup values
    /// </summary>
    [JsonIgnore]
    List<LookupObject> IFieldObject.LookupValues => null;
    
    [JsonIgnore]
    public CultureInfo TenantCulture { get; set; }

    [JsonIgnore]
    public IValueFormatter ValueFormatter { get; set; }

    [JsonIgnore]
    bool IFieldObject.ValueMandatory => Mandatory;

    bool IFieldObject.IsValidValue() => true;

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

    [JsonIgnore]
    public string Culture => null;

    public string GetLocalizedName(CultureInfo culture) =>
        culture.Name.GetLocalization(NameLocalizations, Name);

    public string GetLocalizedDescription(CultureInfo culture) =>
        culture.Name.GetLocalization(DescriptionLocalizations, Description);

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

    public bool EqualKey(ReportParameter compare) =>
        base.EqualKey(compare);
}