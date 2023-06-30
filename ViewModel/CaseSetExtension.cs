using PayrollEngine.Client.Model;

namespace PayrollEngine.WebApp.ViewModel;

public static class CaseSetExtension
{
    public static CaseSetup ToCaseChangeSetup(this CaseSet caseSet, bool submitMode)
    {
        var caseSetup = new CaseSetup();
        CollectCaseSetup(caseSet, caseSetup, submitMode);
        return caseSetup;
    }

    // recursive case value collector
    private static void CollectCaseSetup(CaseSet caseSet, CaseSetup caseSetup, bool submitMode)
    {
        if (caseSet == null)
        {
            return;
        }

        caseSetup.CaseName = caseSet.Name;
        caseSetup.CaseSlot = caseSet.CaseSlot;

        // case values
        if (caseSet.Fields != null)
        {
            caseSetup.Values = new();
            foreach (var field in caseSet.Fields)
            {
                var caseValue = new CaseValueSetup(caseSet.Name, field);

                // ensure backend UTC times
                if (caseValue.Start.HasValue)
                {
                    caseValue.Start = caseValue.Start.Value.SpecifyUtc();
                }
                if (caseValue.End.HasValue)
                {
                    caseValue.End = caseValue.End.Value.SpecifyUtc();
                }

                // when creating values to submit to backend, transform object differently
                if (submitMode && field.Input is IMaskedInput maskedInput)
                {
                    // Special case for masked input
                    // in UI it needs to be display without mask, in DB it must be saved with mask
                    if (maskedInput.TryGetMaskedValue(out var maskedValue))
                    {
                        caseValue.Value = maskedValue;
                    }
                }

                // create case documents from viewmodel
                if (field.Documents != null)
                {
                    caseValue.Documents = new();
                    foreach (var document in field.Documents)
                    {
                        caseValue.Documents.Add(new()
                        {
                            Name = document.Name,
                            Content = document.Content,
                            ContentType = document.ContentType
                        });
                    }
                }

                caseSetup.Values.Add(caseValue);
            }
        }

        // related cases
        if (caseSet.RelatedCases != null)
        {
            caseSetup.RelatedCases = new();
            foreach (var relatedCase in caseSet.RelatedCases)
            {
                var relatedCaseSetup = new CaseSetup();
                caseSetup.RelatedCases.Add(relatedCaseSetup);
                CollectCaseSetup(relatedCase, relatedCaseSetup, submitMode);
            }
        }
    }
}