using System.Globalization;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using PayrollEngine.WebApp.Shared;
using PayrollEngine.WebApp.ViewModel;
using Task = System.Threading.Tasks.Task;

namespace PayrollEngine.WebApp.Presentation.Field;

public abstract class FieldEditorBase : ComponentBase
{
    [Parameter] public IFieldObject Field { get; set; }
    [Parameter] public CultureInfo Culture { get; set; }
    [Parameter] public Variant Variant { get; set; }
    [Parameter] public string HelperText { get; set; }
    [Parameter] public bool Disabled { get; set; }
    [Inject] private ILocalizerService LocalizerService { get; set; }

    protected Localizer Localizer => LocalizerService.Localizer;
    protected bool Error => !Field.IsValidValue();

    protected string ValueHelp { get; private set; }
    protected string ValueAdornmentText { get; private set; }
    protected Adornment ValueAdornment { get; private set; }
    protected string ValueLabel { get; private set; }
    protected string ValueRequiredError { get; private set; }
    protected bool ReadOnly { get; private set; }

    protected void SetValueLabel(string label)
    {
        if (string.IsNullOrWhiteSpace(label))
        {
            return;
        }
        ValueLabel = label;
    }

    /// <summary>
    /// The default MudCheckBox raises a required error on false value
    /// </summary>
    protected bool Required =>
        !Field.HasValue && Field.ValueMandatory;

    protected override async Task OnParametersSetAsync()
    {
        // culture
        var culture = Field.Attributes.GetCulture(Culture);
        if (!string.IsNullOrWhiteSpace(culture) && !string.Equals(culture, Culture.Name))
        {
            // overwrite parameter value
            Culture = new CultureInfo(culture);
        }

        await base.OnParametersSetAsync();
    }

    protected override async Task OnInitializedAsync()
    {
        // label
        var label = Field.Attributes.GetValueLabel(Culture);
        if (string.IsNullOrWhiteSpace(label))
        {
            label = Field.GetLocalizedName(Culture);
        }
        SetValueLabel(label);

        // help
        ValueHelp = Field.Attributes.GetValueHelp(Culture);
        if (string.IsNullOrWhiteSpace(ValueHelp))
        {
            ValueHelp = Field.GetLocalizedDescription(Culture);
        }

        // adornment
        ValueAdornmentText = Field.Attributes.GetValueAdornment(Culture);
        ValueAdornment = string.IsNullOrWhiteSpace(ValueAdornmentText) ?
            Adornment.None : Adornment.End;

        // data
        ReadOnly = Field.Attributes.GetValueReadOnly(Culture) ?? false;
        ValueRequiredError = Field.Attributes.GetValueRequired(Culture) ??
                             Localizer.Error.MissingMandatoryValue;

        await base.OnInitializedAsync();
    }
}