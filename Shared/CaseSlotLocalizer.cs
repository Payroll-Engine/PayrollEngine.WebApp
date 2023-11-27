using Microsoft.Extensions.Localization;

namespace PayrollEngine.WebApp.Shared;

public class CaseSlotLocalizer(IStringLocalizerFactory factory) : LocalizerBase(factory)
{
    public string CaseSlot => PropertyValue();
    public string CaseSlots => PropertyValue();
    public string NotAvailable => PropertyValue();
    public string Localizations => PropertyValue();
}