using Microsoft.Extensions.Localization;

namespace PayrollEngine.WebApp.Shared;

public class ScriptLocalizer : LocalizerBase
{
    public ScriptLocalizer(IStringLocalizerFactory factory) :
        base(factory)
    {
    }

    public string Script => FromCaller();
    public string Scripts => FromCaller();

    public string FunctionTypes => FromCaller();
    public string SingleFunction => FromCaller();

    public string FunctionCount(int count) =>
        string.Format(FromCaller(), count);
}