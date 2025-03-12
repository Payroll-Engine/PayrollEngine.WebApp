using System;
using System.Linq;
using System.Collections;
using PayrollEngine.WebApp.ViewModel;

namespace PayrollEngine.WebApp.Presentation.Regulation;

/// <summary>
/// Regulation item value
/// </summary>
public class RegulationItemValue
{
    private IRegulationItem Item { get; }
    private object Value { get; }
    private string ValueFormat { get; }

    /// <summary>
    /// Default constructor
    /// </summary>
    public RegulationItemValue()
    {
    }

    /// <summary>
    /// Value constructor
    /// </summary>
    /// <param name="item">Regulation item</param>
    /// <param name="value">Item value</param>
    /// <param name="valueFormat">Value format</param>
    public RegulationItemValue(IRegulationItem item, object value, string valueFormat = null)
    {
        Item = item;
        Value = value;
        ValueFormat = valueFormat;
    }

    /// <summary>
    /// Item object id
    /// </summary>
    public string ObjectId =>
        Item?.Id.ToString();

    /// <summary>
    /// Item text
    /// </summary>
    public string Text
    {
        get
        {
            var value = Value?.ToString();
            var valueType = Value?.GetType();

            if (valueType != null && Value is IList list)
            {
                var values = list.Cast<object>().ToList();
                value = values.Count > 0 ? string.Join(",", values) : null;
            }
            else if (!string.IsNullOrWhiteSpace(ValueFormat))
            {
                value = string.Format(ValueFormat, Value);
            }
            return $"{Item?.RegulationName}: {value}";
        }
    }

    /// <summary>
    /// Item value
    /// </summary>
    public T GetValue<T>() =>
        (T)Convert.ChangeType(Value, typeof(T));
}