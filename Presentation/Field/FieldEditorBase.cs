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
    [Parameter] public string Culture { get; set; }
    [Parameter] public Variant Variant { get; set; }
    [Parameter] public string HelperText { get; set; }
    [Parameter] public bool Disabled { get; set; }
    [Inject] private Localizer Localizer { get; set; }

    protected bool Error => !Field.IsValidValue();

    protected virtual string ValueHelp { get; set; }
    protected virtual string ValueAdornmentText { get; set; }
    protected virtual Adornment ValueAdornment { get; set; }
    protected virtual string ValueLabel { get; set; }
    protected virtual string ValueRequiredError { get; set; }
    protected virtual bool ReadOnly { get; set; }

    /// <summary>
    /// The default MudCheckBox raises a required error on false value
    /// </summary>
    protected bool Required =>
        !Field.HasValue && Field.ValueMandatory;

    // culture
    private CultureInfo culture;
    protected virtual CultureInfo CultureInfo
    {
        get
        {
            if (culture == null)
            {
                var customCulture = Field.Attributes.GetCulture(Culture);
                culture = string.IsNullOrWhiteSpace(customCulture) ?
                    Field.ValueFormatter.Culture :
                    CultureTool.GetCulture(customCulture);
            }
            return culture;
        }
    }

    protected override async Task OnInitializedAsync()
    {
        // label
        ValueLabel = Field.Attributes.GetValueLabel(Culture);
        if (string.IsNullOrWhiteSpace(ValueLabel))
        {
            ValueLabel = Field.GetLocalizedName(Culture);
        }

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