using PayrollEngine.WebApp.Shared;

namespace PayrollEngine.WebApp;

public static class RegulationItemTypeExtensions
{
    public static string LocalizedName(this RegulationItemType itemType, Localizer localizer, bool plural = false)
    {
        switch (itemType)
        {
            case RegulationItemType.Case:
                return plural ? localizer.Case.Cases : localizer.Case.Case;
            case RegulationItemType.CaseField:
                return plural ? localizer.CaseField.CaseFields : localizer.CaseField.CaseField;
            case RegulationItemType.CaseRelation:
                return plural ? localizer.CaseRelation.CaseRelations : localizer.CaseRelation.CaseRelation;
            case RegulationItemType.Collector:
                return plural ? localizer.Collector.Collectors : localizer.Collector.Collector;
            case RegulationItemType.WageType:
                return plural ? localizer.WageType.WageTypes : localizer.WageType.WageType;
            case RegulationItemType.Report:
                return plural ? localizer.Report.Reports : localizer.Report.Report;
            case RegulationItemType.ReportParameter:
                return plural ? localizer.ReportParameter.ReportParameters : localizer.ReportParameter.ReportParameter;
            case RegulationItemType.ReportTemplate:
                return plural ? localizer.ReportTemplate.ReportTemplates : localizer.ReportTemplate.ReportTemplate;
            case RegulationItemType.Lookup:
                return plural ? localizer.Lookup.Lookups : localizer.Lookup.Lookup;
            case RegulationItemType.LookupValue:
                return plural ? localizer.LookupValue.LookupValues : localizer.LookupValue.LookupValue;
            case RegulationItemType.Script:
                return plural ? localizer.Script.Scripts : localizer.Script.Script;
            default:
                return itemType.ToString();
        }
    }

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
                throw new PayrollException($"Regulation item type {itemType} has no parent.");
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