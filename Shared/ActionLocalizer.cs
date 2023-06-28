using Microsoft.Extensions.Localization;

namespace PayrollEngine.WebApp.Shared;

public class ActionLocalizer : LocalizerBase
{
    public ActionLocalizer(IStringLocalizerFactory factory) :
        base(factory)
    {
    }

    public string Action => FromCaller();
    public string Actions => FromCaller();
    public string NotAvailable => FromCaller();

    public string MoveUp => FromCaller();
    public string MoveDown => FromCaller();

    public string ActionExpression => FromCaller();
    public string Parameters => FromCaller();
    public string Issues => FromCaller();
    public string Source => FromCaller();
    public string Namespace => FromCaller();
    public string Categories => FromCaller();
    public string AppendAction => FromCaller();
}