using System;
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
    bool HasValue { get; }

    /// <summary>
    /// The value as string
    /// </summary>
    string ValueAsString { get; set; }

    /// <summary>
    /// The value as integer
    /// </summary>
    int? ValueAsInteger { get; set; }

    /// <summary>
    /// The value as boolean
    /// </summary>
    bool? ValueAsBoolean { get; set; }

    /// <summary>
    /// The value as decimal
    /// </summary>
    decimal? ValueAsDecimal { get; set; }

    /// <summary>
    /// The value as percent
    /// </summary>
    decimal? ValueAsPercent { get; set; }

    /// <summary>
    /// The value as date time
    /// </summary>
    DateTime? ValueAsDateTime { get; set; }
}