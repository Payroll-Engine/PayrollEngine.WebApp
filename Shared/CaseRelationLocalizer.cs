using Microsoft.Extensions.Localization;

namespace PayrollEngine.WebApp.Shared;

public class CaseRelationLocalizer : LocalizerBase
{
    public CaseRelationLocalizer(IStringLocalizerFactory factory) :
        base(factory)
    {
    }

    public string Relation => FromCaller();
    public string CaseRelation => FromCaller();
    public string CaseRelations => FromCaller();

    public string SourceCaseName => FromCaller();
    public string SourceCaseSlot => FromCaller();
    public string TargetCaseName => FromCaller();
    public string TargetCaseSlot => FromCaller();

    public string BuildExpression => FromCaller();
    public string ValidateExpression => FromCaller();

    public string BuildActions => FromCaller();
    public string ValidateActions => FromCaller();
}