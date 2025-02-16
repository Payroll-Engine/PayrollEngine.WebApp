using Microsoft.Extensions.Localization;
using System.Globalization;

namespace PayrollEngine.WebApp.Shared;

public class ScriptLocalizer(IStringLocalizerFactory factory, CultureInfo culture) : 
    LocalizerBase(factory, culture: culture)
{
    public string Script => PropertyValue();
    public string Scripts => PropertyValue();

    public string FunctionTypes => PropertyValue();
    public string SingleFunction => PropertyValue();

    public string FunctionCount(int count) =>
        FormatValue(PropertyValue(), nameof(count), count);
}