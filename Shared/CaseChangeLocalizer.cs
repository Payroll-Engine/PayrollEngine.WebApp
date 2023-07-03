﻿using Microsoft.Extensions.Localization;

namespace PayrollEngine.WebApp.Shared;

public class CaseChangeLocalizer : LocalizerBase
{
    public CaseChangeLocalizer(IStringLocalizerFactory factory) :
        base(factory)
    {
    }

    public string CaseChange => PropertyValue();
    public string CaseChanges => PropertyValue();
    public string NotAvailable => PropertyValue();
    public string Undo => PropertyValue();
    public string Reason => PropertyValue();
    public string UndoCaseChange => PropertyValue();
    public string EmptyCaseChange => PropertyValue();

    public string UndoError(string @case) =>
        FormatValue(PropertyValue(), nameof(@case), @case);
    public string UndoSuccess(string @case) =>
        FormatValue(PropertyValue(), nameof(@case), @case);
    public string UndoQuery(string @case) =>
        FormatValue(PropertyValue(), nameof(@case), @case);
    public string CaseChangeGroup(object group) =>
        FormatValue(PropertyValue(), nameof(group), group);
    public string UndoCancelGroup(string date) =>
        FormatValue(PropertyValue(), nameof(date), date);
}