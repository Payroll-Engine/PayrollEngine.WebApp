using System.Linq;
using System.Globalization;
using System.ComponentModel;
using Microsoft.AspNetCore.Components;

namespace PayrollEngine.WebApp.ViewModel;

/// <summary>
/// Extension methods for <see cref="IFieldObject" />
/// </summary>
public static class FieldObjectExtensions
{
    /// <summary>
    /// Get value markup
    /// </summary>
    /// <param name="fieldObject">Field object</param>
    /// <param name="culture">Culture</param>
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
                return new(fieldObject.FormatValue(culture).Replace("\n", "<br />"));
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
            return new(HtmlTool.BuildWebLink(address));
        }

        // money
        if (fieldObject.ValueType == ValueType.Money)
        {
            var valueAsDecimal = fieldObject.ValueAsDecimal;
            if (valueAsDecimal.HasValue)
            {
                return new(fieldObject.FormatValue(culture));
            }
        }

        // percent
        if (fieldObject.ValueType == ValueType.Percent)
        {
            var valueAsPercent = fieldObject.ValueAsPercent;
            if (valueAsPercent.HasValue)
            {
                return new(fieldObject.FormatValue(culture));
            }
        }

        // other values
        return new(fieldObject.FormatValue(culture));
    }
}