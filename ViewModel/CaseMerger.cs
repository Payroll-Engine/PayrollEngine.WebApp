using System;
using System.Collections.Generic;
using System.Linq;

namespace PayrollEngine.WebApp.ViewModel;

public static class CaseMerger
{
    public static async System.Threading.Tasks.Task MergeAsync(CaseSet changeCaseSet, CaseSet targetCaseSet)
    {
        if (changeCaseSet == null)
        {
            throw new ArgumentNullException(nameof(changeCaseSet));
        }
        if (targetCaseSet == null)
        {
            throw new ArgumentNullException(nameof(targetCaseSet));
        }

        // fields
        await MergeFieldsAsync(changeCaseSet, targetCaseSet);

        // relations
        if (changeCaseSet.RelatedCases == null || !changeCaseSet.RelatedCases.Any())
        {
            // clear any 
            targetCaseSet.RelatedCases?.ClearAsync();
            targetCaseSet.RelatedCases = null;
            return;
        }
        if (targetCaseSet.RelatedCases == null)
        {
            // initial add
            targetCaseSet.RelatedCases = [];
            await targetCaseSet.RelatedCases.AddRangeAsync(changeCaseSet.RelatedCases);
            return;
        }

        var sourceCases = new List<CaseSet>(changeCaseSet.RelatedCases);
        var targetCases = new List<CaseSet>(targetCaseSet.RelatedCases);
        if (changeCaseSet.RelatedCases != null)
        {
            foreach (var sourceCase in sourceCases)
            {
                // find target case
                var targetCase = targetCases.FirstOrDefault(
                    x => Equals(x.Name, sourceCase.Name) && string.Equals(x.CaseSlot, sourceCase.CaseSlot));
                if (targetCase != null)
                {
                    await MergeFieldsAsync(sourceCase, targetCase);

                    // remove mapped case
                    targetCases.Remove(targetCase);

                    // recursive merge
                    await MergeAsync(sourceCase, targetCase);
                }
                else
                {
                    // append new related case
                    await targetCaseSet.RelatedCases.AddAsync(sourceCase);
                }
            }
        }

        // cleanup remaining related cases
        foreach (var targetCase in targetCases)
        {
            await targetCaseSet.RelatedCases.RemoveAsync(targetCase);
        }
    }

    private static async System.Threading.Tasks.Task MergeFieldsAsync(CaseSet sourceCaseSet, CaseSet targetCaseSet)
    {
        if (sourceCaseSet.Fields == null || !sourceCaseSet.Fields.Any())
        {
            // clear all fields
            targetCaseSet.Fields?.ClearAsync();
            return;
        }

        var sourceFields = new List<CaseFieldSet>(sourceCaseSet.Fields);
        var targetFields = new List<CaseFieldSet>(targetCaseSet.Fields);

        // copy values to existing
        foreach (var sourceField in sourceFields)
        {
            var targetField = targetFields.FirstOrDefault(x => string.Equals(x.Name, sourceField.Name));
            if (targetField != null)
            {
                // start
                if (sourceField.Start != targetField.Start)
                {
                    targetField.Start = sourceField.Start;
                }

                // end
                if (sourceField.End != targetField.End)
                {
                    targetField.End = sourceField.End;
                }

                // value
                if (sourceField.HasValue && sourceField.Value != targetField.Value)
                {
                    targetField.Value = sourceField.Value;
                }

                // cancellation
                if (sourceField.CancellationDate != targetField.CancellationDate)
                {
                    targetField.CancellationDate = sourceField.CancellationDate;
                }

                // attributes
                if (!sourceField.Attributes.ContentEquals(targetField.Attributes))
                {
                    targetField.Attributes = sourceField.Attributes;
                }

                // remove mapped fields
                targetFields.RemoveAll(x => string.Equals(x.Name, sourceField.Name));
            }
            else
            {
                // append new case field
                await targetCaseSet.Fields.AddAsync(sourceField);
            }
        }

        // cleanup remaining target fields
        foreach (var targetField in targetFields)
        {
            await targetCaseSet.Fields.RemoveAllAsync(x => string.Equals(x.Name, targetField.Name));
        }
    }
}