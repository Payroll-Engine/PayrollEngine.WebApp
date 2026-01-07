using System.Globalization;
using Microsoft.Extensions.Localization;

namespace PayrollEngine.WebApp.Shared;

public class ActionLocalizer(IStringLocalizerFactory factory, CultureInfo culture) : 
    LocalizerBase(factory, culture: culture)
{
    public string Action => PropertyValue();
    public string Actions => PropertyValue();
    public string NotAvailable => PropertyValue();

    public string MoveUp => PropertyValue();
    public string MoveDown => PropertyValue();

    public string ActionSyntax => PropertyValue();
    public string ActionSyntaxHelp => PropertyValue();
    public string Parameters => PropertyValue();
    public string Issues => PropertyValue();
    public string Source => PropertyValue();
    public string Categories => PropertyValue();
    public string AppendAction => PropertyValue();
    public string Marker => PropertyValue();
    public string ShowActions => PropertyValue();
    public string LoadingActions => PropertyValue();
    public string HideActions => PropertyValue();
}