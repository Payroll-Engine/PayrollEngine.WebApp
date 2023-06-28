﻿using System.Collections.Generic;
using System.Globalization;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using PayrollEngine.WebApp.ViewModel;
using Task = System.Threading.Tasks.Task;

namespace PayrollEngine.WebApp.Presentation.Field;

public abstract class DateTimePickerBase : ComponentBase, IAttributeObject
{
    [Parameter] public CaseFieldSet Field { get; set; }
    [Parameter] public Dictionary<string, object> Attributes { get; set; }
    [Parameter] public string Culture { get; set; }
    [Parameter] public string Class { get; set; }
    [Parameter] public string Style { get; set; }
    [Parameter] public bool Dense { get; set; }
    [Parameter] public Variant Variant { get; set; }

    protected abstract DatePickerType DatePickerType { get; }
    protected abstract DateTimeType DateTimeType { get; }

    protected virtual OpenTo OpenTo { get; set; }

    // culture
    private CultureInfo culture;
    protected virtual CultureInfo CultureInfo => 
        culture ??= CultureTool.GetCulture(Culture);

    protected override async Task OnInitializedAsync()
    {
        switch (DatePickerType)
        {
            case DatePickerType.Month:
                OpenTo = OpenTo.Year;
                break;
            case DatePickerType.Year:
                OpenTo = OpenTo.Year;
                break;
            case DatePickerType.Day:
                OpenTo = OpenTo.Date;
                break;
        }
        await base.OnInitializedAsync();
    }
}