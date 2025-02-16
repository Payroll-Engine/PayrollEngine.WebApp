using Microsoft.Extensions.Localization;
using System.Globalization;

namespace PayrollEngine.WebApp.Shared;

public class ActionLocalizer(IStringLocalizerFactory factory, CultureInfo culture) : 
    LocalizerBase(factory, culture: culture)
{
    public string Action => PropertyValue();
    public string Actions => PropertyValue();
    public string NotAvailable => PropertyValue();

    public string MoveUp => PropertyValue();
    public string MoveDown => PropertyValue();

    public string ActionExpression => PropertyValue();
    public string Parameters => PropertyValue();
    public string Issues => PropertyValue();
    public string Source => PropertyValue();
    public string Namespace => PropertyValue();
    public string Categories => PropertyValue();
    public string AppendAction => PropertyValue();
}