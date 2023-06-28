using PayrollEngine.WebApp.Shared;

namespace PayrollEngine.WebApp.ViewModel;

public static class RegulationItemExtensions
{
    public static string GetAdditionalInfo(this IRegulationItem item, Localizer localizer, int length) =>
        item.GetAdditionalInfo(localizer).TruncateSentence(length);
}