using System.Globalization;

namespace PayrollEngine.WebApp;

public class CultureDescription(CultureInfo cultureInfo)
{
    public CultureInfo CultureInfo { get; } = cultureInfo;
    public string Name => CultureInfo.Name;
    private string DisplayName => CultureInfo.DisplayName;

    public override string ToString() =>
        $"{Name} - {DisplayName}";
}