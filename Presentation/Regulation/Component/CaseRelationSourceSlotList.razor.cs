using System;
using PayrollEngine.WebApp.ViewModel;

namespace PayrollEngine.WebApp.Presentation.Regulation.Component;

public partial class CaseRelationSourceSlotList
{
    protected override string GetSlotCaseName()
    {
        if (Item is not RegulationCaseRelation caseRelation)
        {
            throw new InvalidOperationException();
        }
        return caseRelation.SourceCaseName;
    }
}
