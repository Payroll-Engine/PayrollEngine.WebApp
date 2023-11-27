using Microsoft.Extensions.Localization;

namespace PayrollEngine.WebApp.Shared;

public class ScriptLocalizer(IStringLocalizerFactory factory) : LocalizerBase(factory)
{
    public string Script => PropertyValue();
    public string Scripts => PropertyValue();

    public string FunctionTypes => PropertyValue();
    public string SingleFunction => PropertyValue();

    public string FunctionCount(int count) =>
        FormatValue(PropertyValue(), nameof(count), count);
}