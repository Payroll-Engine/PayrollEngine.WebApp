using System;
using MudBlazor;

namespace PayrollEngine.WebApp.Server.Components.Pages;

public class TreeCaseSet(ViewModel.CaseSet caseSet) : TreeItemData<ViewModel.CaseSet>
{
    public ViewModel.CaseSet CaseSet { get; } = caseSet ?? throw new ArgumentNullException(nameof(caseSet));

    public ObservedHashSet<TreeCaseSet> RelatedCases
    {
        get
        {
            var treeRelatedCases = new ObservedHashSet<TreeCaseSet>();
            if (CaseSet?.RelatedCases != null)
            {
                foreach (var relatedCase in CaseSet.RelatedCases)
                {
                    treeRelatedCases.Add(new(relatedCase));
                }
            }
            return treeRelatedCases;
        }
    }
}