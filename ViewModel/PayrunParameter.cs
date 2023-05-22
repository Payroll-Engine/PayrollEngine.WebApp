using System;
using System.Collections.Generic;
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
    public bool HasValue => !string.IsNullOrWhiteSpace(Value);

    /// <inheritdoc />
    [JsonIgnore]
    public string ValueAsString
    {
        get => string.IsNullOrWhiteSpace(Value) ? null : ValueConvert.ToString(Value);
        set => Value = ValueConvert.ToJson(value);
    }

    /// <inheritdoc />
    [JsonIgnore]
    public int? ValueAsInteger
    {
        get => string.IsNullOrWhiteSpace(Value) ? null : ValueConvert.ToInteger(Value);
        set => Value = ValueConvert.ToJson(value);
    }

    /// <inheritdoc />
    [JsonIgnore]
    public bool? ValueAsBoolean
    {
        get => !string.IsNullOrWhiteSpace(Value) && ValueConvert.ToBoolean(Value);
        set => Value = ValueConvert.ToJson(value);
    }

    /// <inheritdoc />
    [JsonIgnore]
    public decimal? ValueAsDecimal
    {
        get => string.IsNullOrWhiteSpace(Value) ? null : ValueConvert.ToDecimal(Value);
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
        get => Date.Parse(Value) ??
               (string.IsNullOrWhiteSpace(Value) ? null : ValueConvert.ToDateTime(Value));
        set => Value = ValueConvert.ToJson(value);
    }

    public string GetLocalizedName(Language language) =>
        language.GetLocalization(NameLocalizations, Name);

    public string GetLocalizedDescription(Language language) =>
        language.GetLocalization(DescriptionLocalizations, Description);

    #endregion

    public override string ToString() =>
        $"{Name}={Value}";
}