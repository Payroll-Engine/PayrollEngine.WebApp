using System;
using System.Collections;
using System.Linq;
using PayrollEngine.WebApp.ViewModel;

namespace PayrollEngine.WebApp.Presentation.Regulation;

public class RegulationItemValue
{
    public IRegulationItem Item { get; set; }
    public object Value { get; set; }
    public string ValueFormat { get; set; }

    public RegulationItemValue()
    {
    }

    public RegulationItemValue(IRegulationItem item, object value, string valueFormat = null)
    {
        Item = item;
        Value = value;
        ValueFormat = valueFormat;
    }

    public string ObjectId =>
        Item?.Id.ToString();

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

    public T GetValue<T>() =>
        (T)Convert.ChangeType(Value, typeof(T));
}