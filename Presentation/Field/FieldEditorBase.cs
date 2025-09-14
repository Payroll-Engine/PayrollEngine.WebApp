using System.Globalization;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using PayrollEngine.WebApp.Shared;
using PayrollEngine.WebApp.ViewModel;
using Task = System.Threading.Tasks.Task;

namespace PayrollEngine.WebApp.Presentation.Field;

/// <summary>
/// Filed editor base class
/// </summary>
public abstract class FieldEditorBase : ComponentBase
{
    /// <summary>
    /// Edit filed
    /// </summary>
    [Parameter] public IFieldObject Field { get; set; }

    /// <summary>
    /// Edit culture
    /// </summary>
    [Parameter] public CultureInfo Culture { get; set; }

    /// <summary>
    /// Input help text
    /// </summary>
    [Parameter] public string HelperText { get; set; }

    /// <summary>
    /// Disabled input
    /// </summary>
    [Parameter] public bool Disabled { get; set; }

    /// <summary>
    /// Localizer service
    /// </summary>
    [Inject] private ILocalizerService LocalizerService { get; set; }

    /// <summary>
    /// Localizer
    /// </summary>
    protected Localizer Localizer => LocalizerService.Localizer;

    /// <summary>
    /// Currency symbol
    /// </summary>
    protected string CurrencySymbol => Culture?.NumberFormat.CurrencySymbol;


    /// <summary>
    /// Value label
    /// </summary>
    protected string ValueLabel { get; private set; }

    /// <summary>
    /// Value help
    /// </summary>
    protected string ValueHelp { get; private set; }

    /// <summary>
    /// Value adornment text
    /// </summary>
    protected string ValueAdornmentText { get; private set; }

    /// <summary>
    /// Value adornment
    /// </summary>
    protected Adornment ValueAdornment { get; private set; }

    /// <summary>
    /// Input variant
    /// </summary>
    protected Variant Variant { get; private set; }

    /// <summary>
    /// Rad only input
    /// </summary>
    protected bool ReadOnly { get; private set; }

    /// <summary>
    /// Required value
    /// </summary>
    protected bool Required => Field.ValueMandatory;

    /// <summary>
    /// Required error message
    /// </summary>
    protected string ValueRequiredError { get; private set; }

    /// <summary>
    /// Field error
    /// </summary>
    protected bool Error => !Field.IsValidValue();

    /// <summary>
    /// Change the value label
    /// </summary>
    /// <param name="label">New label</param>
    protected void SetValueLabel(string label)
    {
        if (string.IsNullOrWhiteSpace(label))
        {
            return;
        }
        ValueLabel = label;
    }

    /// <inheritdoc />
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

        // variant
        var inputVariant = Field.Attributes.GetVariant(Culture);
        if (inputVariant != null)
        {
            Variant = (Variant)inputVariant.Value;
        }

        // data
        ReadOnly = Field.Attributes.GetValueReadOnly(Culture) ?? false;
        ValueRequiredError = Field.Attributes.GetValueRequired(Culture) ??
                                 Localizer.Error.MissingMandatoryValue;

        await base.OnInitializedAsync();
    }
}