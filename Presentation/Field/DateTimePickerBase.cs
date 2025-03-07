﻿using System.Globalization;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using PayrollEngine.WebApp.ViewModel;

namespace PayrollEngine.WebApp.Presentation.Field;

public abstract class DateTimePickerBase : ComponentBase, IAttributeObject
{
    [Parameter] public CaseFieldSet Field { get; set; }
    [Parameter] public Dictionary<string, object> Attributes { get; set; }
    [Parameter] public CultureInfo Culture { get; set; }
    [Parameter] public string Class { get; set; }
    [Parameter] public string Style { get; set; }
    [Parameter] public bool Dense { get; set; }
    [Parameter] public Variant Variant { get; set; }

    protected abstract DatePickerType DatePickerType { get; }
    protected abstract DateTimePickerType DateTimePickerType { get; }

    protected bool HasValueHelp =>
        !string.IsNullOrWhiteSpace(Attributes.GetValueHelp(Culture));
}