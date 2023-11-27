using Microsoft.Extensions.Localization;

namespace PayrollEngine.WebApp.Shared;

public class CaseRelationLocalizer(IStringLocalizerFactory factory) : LocalizerBase(factory)
{
    public string Relation => PropertyValue();
    public string CaseRelation => PropertyValue();
    public string CaseRelations => PropertyValue();

    public string SourceCaseName => PropertyValue();
    public string SourceCaseSlot => PropertyValue();
    public string TargetCaseName => PropertyValue();
    public string TargetCaseSlot => PropertyValue();

    public string BuildExpression => PropertyValue();
    public string ValidateExpression => PropertyValue();

    public string BuildActions => PropertyValue();
    public string ValidateActions => PropertyValue();
}