using System;
using System.Linq;
using System.Collections.Generic;

namespace PayrollEngine.WebApp.ViewModel;

/// <summary>
/// Report merger
/// </summary>
public static class ReportMerger
{
    /// <summary>
    /// Merge reports
    /// </summary>
    /// <param name="sourceReportSet">Source report</param>
    /// <param name="targetReportSet">Target report</param>
    public static void Merge(ReportSet sourceReportSet, ReportSet targetReportSet)
    {
        if (sourceReportSet == null)
        {
            throw new ArgumentNullException(nameof(sourceReportSet));
        }

        if (targetReportSet == null)
        {
            throw new ArgumentNullException(nameof(targetReportSet));
        }

        // parameters and templates
        targetReportSet.ApplyReportSet(sourceReportSet);

        // report attributes
        targetReportSet.Attributes = MergeAttributes(sourceReportSet.Attributes, targetReportSet.Attributes);

        // parameters
        MergeParameters(sourceReportSet, targetReportSet);
    }

    /// <summary>
    /// Merge report parameters
    /// </summary>
    /// <param name="sourceReportSet">Source report</param>
    /// <param name="targetReportSet">Target report</param>
    private static void MergeParameters(ReportSet sourceReportSet, ReportSet targetReportSet)
    {
        if (sourceReportSet.Parameters == null || !sourceReportSet.Parameters.Any())
        {
            // clear all fields
            targetReportSet.Parameters?.Clear();
            return;
        }

        var sourceParameters = new List<Client.Model.ReportParameter>(sourceReportSet.Parameters);
        var targetParameters = new List<Client.Model.ReportParameter>(targetReportSet.Parameters);
        if (sourceParameters.Count != targetParameters.Count)
        {
            throw new PayrollException($"Report with unbalanced parameter count: {sourceParameters.Count} and {targetParameters.Count}");
        }

        // report parameter values
        foreach (var sourceParameter in sourceParameters)
        {
            var targetParameter = targetParameters.FirstOrDefault(x => string.Equals(x.Name, sourceParameter.Name));
            if (targetParameter != null)
            {
                targetParameter.Hidden = sourceParameter.Hidden;
                targetParameter.Mandatory = sourceParameter.Mandatory;
                targetParameter.Value = sourceParameter.Value;
                targetParameter.Attributes = MergeAttributes(sourceParameter.Attributes, targetParameter.Attributes);
            }
        }
    }

    private static Dictionary<string, object> MergeAttributes(
        Dictionary<string, object> sourceDictionary,
        Dictionary<string, object> targetDictionary)
    {
        if (sourceDictionary == null)
        {
            return null;
        }
        return sourceDictionary.ContentEquals(targetDictionary) ?
            sourceDictionary :
            new Dictionary<string, object>(sourceDictionary);
    }
}