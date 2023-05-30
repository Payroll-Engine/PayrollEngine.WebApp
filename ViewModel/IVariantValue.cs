using System;
using System.Text.Json.Serialization;

namespace PayrollEngine.WebApp.ViewModel;

public interface IVariantValue
{
    /// <summary>
    /// The value type
    /// </summary>
    ValueType ValueType { get; set; }

    /// <summary>
    /// The json value
    /// </summary>
    string Value { get; set; }

    /// <summary>
    /// Test if value is present
    /// </summary>
    [JsonIgnore]
    bool HasValue { get; }

    /// <summary>
    /// The value as string
    /// </summary>
    [JsonIgnore]
    string ValueAsString { get; set; }

    /// <summary>
    /// The value as integer
    /// </summary>
    [JsonIgnore]
    int? ValueAsInteger { get; set; }

    /// <summary>
    /// The value as boolean
    /// </summary>
    [JsonIgnore]
    bool? ValueAsBoolean { get; set; }

    /// <summary>
    /// The value as decimal
    /// </summary>
    [JsonIgnore]
    decimal? ValueAsDecimal { get; set; }

    /// <summary>
    /// The value as percent
    /// </summary>
    [JsonIgnore]
    decimal? ValueAsPercent { get; set; }

    /// <summary>
    /// The value as date time
    /// </summary>
    [JsonIgnore]
    DateTime? ValueAsDateTime { get; set; }
}