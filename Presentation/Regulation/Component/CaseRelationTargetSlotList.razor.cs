using System;
using PayrollEngine.WebApp.ViewModel;

namespace PayrollEngine.WebApp.Presentation.Regulation.Component;

public partial class CaseRelationTargetSlotList
{
    protected override string GetSlotCaseName()
    {
        if (Item is not RegulationCaseRelation caseRelation)
        {
            throw new InvalidOperationException();
        }
        return caseRelation.TargetCaseName;
    }
}
