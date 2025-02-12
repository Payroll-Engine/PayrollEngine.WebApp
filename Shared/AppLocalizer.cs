using Microsoft.Extensions.Localization;

namespace PayrollEngine.WebApp.Shared;

public class AppLocalizer(IStringLocalizerFactory factory) : LocalizerBase(factory)
{
    public string SelectTenant => PropertyValue();
    public string SelectPayroll => PropertyValue();
    public string SelectPayrollWithPayrun => PropertyValue();
    public string SelectEmployee => PropertyValue();
    public string SelectPayrun => PropertyValue();

    public string AccessDenied => PropertyValue();
    public string MissingFeatures => PropertyValue();
    public string ContactEmail=> PropertyValue();
    public string AdminContactError => PropertyValue();

    public string ToggleSidebar => PropertyValue();
    public string LightMode => PropertyValue();
    public string DarkMode => PropertyValue();

    public string About => PropertyValue();
    public string Logout => PropertyValue();

    public string CompactView => PropertyValue();
    public string ExpandGroups => PropertyValue();
    public string CollapseGroups => PropertyValue();

    public string WebAppVersion(string version) =>
        FormatValue(PropertyValue(), nameof(version), version);
    public string BackendVersion(string version) =>
        FormatValue(PropertyValue(), nameof(version), version);

    public string Copyright(string owner) => FormatValue(PropertyValue(), nameof(owner), owner);
}