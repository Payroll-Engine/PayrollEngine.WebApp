using System;

namespace PayrollEngine.WebApp;

/// <summary>
/// Interface to format values
/// </summary>
public interface IValueFormatter
{
    /// <summary>
    /// Convert a json string value to string
    /// </summary>
    /// <param name="json">The JSON value</param>
    /// <param name="valueType">The type of the value</param>
    string ToString(string json, ValueType valueType);

    /// <summary>
    /// Convert an object value to string
    /// </summary>
    /// <param name="value">The value</param>
    /// <param name="valueType">The type of the value</param>
    string ToString(object value, ValueType valueType);

    /// <summary>
    /// Convert a date time value to a compact string
    /// </summary>
    /// <param name="date">The date value</param>
    string ToCompactString(DateTime? date);
}
