using System.Globalization;

namespace PayrollEngine.WebApp;

/// <summary>
/// Culture description
/// </summary>
/// <param name="cultureInfo"></param>
public class CultureDescription(CultureInfo cultureInfo)
{
    /// <summary>
    /// Culture info
    /// </summary>
    public CultureInfo CultureInfo { get; } = cultureInfo;

    /// <summary>
    /// Culture name
    /// </summary>
    public string Name => CultureInfo.Name;

    /// <inheritdoc />
    public override string ToString() =>
        $"{Name} - {CultureInfo.DisplayName}";
}