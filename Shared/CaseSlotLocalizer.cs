using Microsoft.Extensions.Localization;

namespace PayrollEngine.WebApp.Shared;

public class CaseSlotLocalizer : LocalizerBase
{
    public CaseSlotLocalizer(IStringLocalizerFactory factory) :
        base(factory)
    {
    }

    public string CaseSlot => PropertyValue();
    public string CaseSlots => PropertyValue();
    public string NotAvailable => PropertyValue();
    public string Localizations => PropertyValue();
}