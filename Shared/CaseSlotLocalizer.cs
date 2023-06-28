using Microsoft.Extensions.Localization;

namespace PayrollEngine.WebApp.Shared;

public class CaseSlotLocalizer : LocalizerBase
{
    public CaseSlotLocalizer(IStringLocalizerFactory factory) :
        base(factory)
    {
    }

    public string CaseSlot => FromCaller();
    public string CaseSlots => FromCaller();
    public string NotAvailable => FromCaller();
    public string Localizations => FromCaller();
}