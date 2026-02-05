using System.Globalization;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using PayrollEngine.WebApp.ViewModel;

namespace PayrollEngine.WebApp.Presentation.Field;

/// <summary>
/// Date time picker base class
/// </summary>
public abstract class DateTimePickerBase : ComponentBase, IAttributeObject
{
    /// <summary>
    /// Case field
    /// </summary>
    [Parameter] public CaseFieldSet Field { get; set; }

    /// <summary>
    /// Case field attributes
    /// </summary>
    [Parameter] public Dictionary<string, object> Attributes { get; set; }

    /// <summary>
    /// Case filed culture
    /// </summary>
    [Parameter] public CultureInfo Culture { get; set; }

    /// <summary>
    /// Input class
    /// </summary>
    [Parameter] public string Class { get; set; }

    /// <summary>
    /// Input style
    /// </summary>
    [Parameter] public string Style { get; set; }

    /// <summary>
    /// Input variant
    /// </summary>
    [Parameter] public Variant Variant { get; set; }

    /// <summary>
    /// Date time picker type
    /// </summary>
    protected abstract DateTimePickerType DateTimePickerType { get; }

    /// <summary>
    /// Test for value help
    /// </summary>
    protected bool HasValueHelp =>
        !string.IsNullOrWhiteSpace(Attributes.GetValueHelp(Culture));
}