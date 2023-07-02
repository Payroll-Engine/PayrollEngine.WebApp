using Microsoft.Extensions.Localization;

namespace PayrollEngine.WebApp.Shared;

public class AppLocalizer : LocalizerBase
{
    public AppLocalizer(IStringLocalizerFactory factory) :
        base(factory)
    {
    }

    public string SelectTenant => FromCaller();
    public string SelectPayroll => FromCaller();
    public string SelectPayrollWithPayrun => FromCaller();
    public string SelectEmployee => FromCaller();
    public string SelectPayrun => FromCaller();

    public string AccessDenied => FromCaller();
    public string MissingFeatures => FromCaller();
    public string AdminContact => FromCaller();

    public string ToggleSidebar => FromCaller();
    public string LightMode => FromCaller();
    public string DarkMode => FromCaller();

    public string About => FromCaller();
    public string Logout => FromCaller();

    public string CompactView => FromCaller();
    public string ExpandGroups => FromCaller();
    public string CollapseGroups => FromCaller();

    public string WebAppVersion => FromCaller();
    public string BackendVersion => FromCaller();

    public string Copyright(string owner) =>
        string.Format(FromCaller(), owner);
}