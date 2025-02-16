using Microsoft.Extensions.Localization;
using System.Globalization;

namespace PayrollEngine.WebApp.Shared;

public class CaseRelationLocalizer(IStringLocalizerFactory factory, CultureInfo culture) : 
    LocalizerBase(factory, culture: culture)
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

    public string SourceEqualsTargetError => PropertyValue();
}