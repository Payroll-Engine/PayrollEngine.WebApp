
namespace PayrollEngine.WebApp.ViewModel;

public static class RegulationItemExtensions
{
    public static string GetAdditionalInfo(this IRegulationItem item, int length) =>
        item.AdditionalInfo.TruncateSentence(length);
}