using Microsoft.Extensions.Localization;
using System.Globalization;

namespace PayrollEngine.WebApp.Shared;

public class CaseSlotLocalizer(IStringLocalizerFactory factory, CultureInfo culture) : 
    LocalizerBase(factory, culture: culture)
{
    public string CaseSlot => PropertyValue();
    public string CaseSlots => PropertyValue();
    public string NotAvailable => PropertyValue();
    public string Localizations => PropertyValue();
}