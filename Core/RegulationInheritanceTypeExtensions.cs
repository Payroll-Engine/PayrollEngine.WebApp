using PayrollEngine.WebApp.Shared;

namespace PayrollEngine.WebApp;

public static class RegulationInheritanceTypeExtensions
{
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