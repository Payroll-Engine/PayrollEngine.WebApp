namespace PayrollEngine.WebApp.ViewModel;

public static class CaseChangeCaseValueExtensions
{
    /// <summary>
    /// Localize slot values
    /// </summary>
    /// <param name="resultItems">List of CaseValue items</param>
    /// <param name="culture">Translation culture</param>
    public static void LocalizeSlot(this CaseChangeCaseValue[] resultItems, string culture)
    {
        foreach (var item in resultItems)
        {
            item.CaseSlot = culture.GetLocalization(item.CaseSlotLocalizations, item.CaseSlot);
        }
    }
}