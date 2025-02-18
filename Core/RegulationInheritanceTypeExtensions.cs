using PayrollEngine.WebApp.Shared;

namespace PayrollEngine.WebApp;

/// <summary>
/// Extension methods for <see cref="RegulationInheritanceType" />
/// </summary>
public static class RegulationInheritanceTypeExtensions
{
    /// <summary>
    /// Convert inheritance type to text
    /// </summary>
    /// <param name="inheritanceType">Inheritance type</param>
    /// <param name="localizer">Localizer</param>
    public static string ToText(this RegulationInheritanceType inheritanceType, Localizer localizer)
    {
        switch (inheritanceType)
        {
            case RegulationInheritanceType.New:
                return localizer.Regulation.InheritanceNew;
            case RegulationInheritanceType.Derived:
                return localizer.Regulation.InheritanceDerived;
            case RegulationInheritanceType.Base:
                return localizer.Regulation.InheritanceBase;
        }
        return null;
    }
}