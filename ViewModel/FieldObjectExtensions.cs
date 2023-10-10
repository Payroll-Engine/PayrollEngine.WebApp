using System.ComponentModel;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Components;

namespace PayrollEngine.WebApp.ViewModel;

public static class FieldObjectExtensions
{
    public static MarkupString GetValueMarkup(this IFieldObject fieldObject, CultureInfo culture)
    {
        // string
        if (fieldObject.ValueType == ValueType.String)
        {
            // masked text
            var mask = fieldObject.Attributes.GetValueMask(culture);
            if (!string.IsNullOrWhiteSpace(mask))
            {
                MaskedTextProvider maskProvider = new(mask);
                maskProvider.Set(fieldObject.ValueAsString);
                return new(maskProvider.ToDisplayString());
            }

            // multi line text
            var lineCount = fieldObject.Attributes.GetLineCount(culture);
            if (lineCount > 1)
            {
                return new(fieldObject.ValueFormatter
                    .ToString(fieldObject.Value, fieldObject.ValueType, culture).Replace("\n", "<br />"));
            }

            // lookup display text
            var lookup = fieldObject.LookupValues?.FirstOrDefault(x => Equals(x.Value, fieldObject.ValueAsString));
            if (lookup != null)
            {
                return new(lookup.Text);
            }
        }

        // resource
        if (fieldObject.ValueType == ValueType.WebResource)
        {
            var address = fieldObject.Value;
            if (string.IsNullOrWhiteSpace(address))
            {
                return new();
            }
            return new($"<a href=\"{fieldObject.Value}\" target=\"_blank\">{fieldObject.Value}</a>");
        }

        // money
        if (fieldObject.ValueType == ValueType.Money)
        {
            var valueAsDecimal = fieldObject.ValueAsDecimal;
            if (valueAsDecimal.HasValue)
            {
                return new(fieldObject.ValueFormatter.ToString(fieldObject.Value, fieldObject.ValueType, culture));
            }
        }

        // other values
        return new(fieldObject.ValueFormatter.ToString(fieldObject.Value, fieldObject.ValueType, culture));
    }

}