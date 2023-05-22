namespace PayrollEngine.WebApp;

public static class RegulationItemTypeExtensions
{
    public static RegulationItemType ParentType(this RegulationItemType itemType)
    {
        switch (itemType)
        {
            case RegulationItemType.CaseField:
                return RegulationItemType.Case;
            case RegulationItemType.ReportParameter:
            case RegulationItemType.ReportTemplate:
                return RegulationItemType.Report;
            case RegulationItemType.LookupValue:
                return RegulationItemType.Lookup;
            default:
                throw new PayrollException($"Regulation item type {itemType} has no parent");
        }
    }

    public static bool HasParentType(this RegulationItemType itemType)
    {
        switch (itemType)
        {
            case RegulationItemType.CaseField:
            case RegulationItemType.ReportParameter:
            case RegulationItemType.ReportTemplate:
            case RegulationItemType.LookupValue:
                return true;
            default:
                return false;
        }
    }
}