using Microsoft.Extensions.Localization;
using System.Globalization;

namespace PayrollEngine.WebApp.Shared;

public class CaseValueLocalizer(IStringLocalizerFactory factory, CultureInfo culture) : 
    LocalizerBase(factory, culture: culture)
{
    public string CaseValue => PropertyValue();
    public string CaseValues => PropertyValue();
    public string NotAvailable => PropertyValue();
}