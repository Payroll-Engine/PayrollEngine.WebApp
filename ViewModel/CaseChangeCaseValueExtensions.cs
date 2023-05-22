namespace PayrollEngine.WebApp.ViewModel;

public static class CaseChangeCaseValueExtensions
{
    /// <summary>
    /// Localize slot values
    /// </summary>
    /// <param name="resultItems">List of CaseValue items</param>
    /// <param name="language">Translation language</param>
    public static void LocalizeSlot(this CaseChangeCaseValue[] resultItems, Language language)
    {
        foreach (var item in resultItems)
        {
            item.CaseSlot = language.GetLocalization(item.CaseSlotLocalizations, item.CaseSlot);
        }
    }
}