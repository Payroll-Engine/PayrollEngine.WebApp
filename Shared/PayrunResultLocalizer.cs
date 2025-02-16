using Microsoft.Extensions.Localization;
using System.Globalization;

namespace PayrollEngine.WebApp.Shared;

public class PayrunResultLocalizer(IStringLocalizerFactory factory, CultureInfo culture) : 
    LocalizerBase(factory, culture: culture)
{
    public string PayrunResult => PropertyValue();
    public string PayrunResults => PropertyValue();
    public string NotAvailable => PropertyValue();

    public string Period => PropertyValue();
    public string ResultKind => PropertyValue();
    public string KindName => PropertyValue();
}