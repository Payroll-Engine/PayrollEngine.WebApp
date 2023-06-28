using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using PayrollEngine.WebApp.Presentation.Component;
using PayrollEngine.WebApp.ViewModel;
using Task = System.Threading.Tasks.Task;

namespace PayrollEngine.WebApp.Presentation.Regulation.Component;

public partial class TextBox : IRegulationInput
{
    [Parameter]
    public RegulationEditContext EditContext { get; set; }
    [Parameter]
    public IRegulationItem Item { get; set; }
    [Parameter]
    public RegulationField Field { get; set; }
    [Parameter]
    public EventCallback<object> ValueChanged { get; set; }
    [Parameter]
    public Variant Variant { get; set; }
    /// <summary>Override field help</summary>
    [Parameter]
    public string HelperText { get; set; }
    [Parameter]
    public string Class { get; set; }
    [Parameter]
    public string Style { get; set; }

    [Inject]
    private IDialogService DialogService { get; set; }

    #region Value

    protected string Value { get; set; }

    protected bool IsBaseValue { get; set; }

    protected string FieldValue
    {
        get => Item.GetPropertyValue<string>(Field.PropertyName);
        set => Item.SetPropertyValue(Field.PropertyName, value);
    }

    private async Task ValueChangedAsync(string item) =>
        await SetFieldValue(item);

    private async Task SetFieldValue(string value)
    {
        // field value
        if (Item.IsNew() || !Field.KeyField)
        {
            var fieldValue = value;
            if (fieldValue != null)
            {
                var baseValue = GetBaseValue();
                if (!string.IsNullOrWhiteSpace(baseValue) &&
                    !Field.Required && string.Equals(fieldValue, baseValue))
                {
                    // reset value on non-mandatory fields
                    fieldValue = null;
                }
            }
            if (string.IsNullOrWhiteSpace(fieldValue))
            {
                fieldValue = null;
            }

            FieldValue = fieldValue;
            ApplyFieldValue();
        }

        // notifications
        UpdateState();
        await ValueChanged.InvokeAsync(value);
    }

    private void ApplyFieldValue()
    {
        // base value
        var value = FieldValue;
        if (value == null && Field.HasBaseValues)
        {
            value = GetBaseValue();
        }
        Value = value;
    }

    protected string GetBaseValue() =>
        Item.GetBaseValue<string>(Field.PropertyName);

    #endregion

    #region Localizations

    private Dictionary<string, string> Localizations { get; set; } = new();

    private bool IsLocalizable =>
        Item.IsLocalizable(Field.PropertyName);

    private Adornment LocalizationAdornment =>
        IsLocalizable ? Adornment.End : Adornment.None;

    private Color LocalizationColor =>
        IsLocalizable && Localizations.Any() ? Color.Tertiary : Color.Primary;

    private async Task OpenLocalizationsAsync()
    {
        if (!IsLocalizable)
        {
            throw new InvalidOperationException($"Property {Field.PropertyName} is not localizable");
        }

        var parameters = new DialogParameters
        {
            { nameof(LocalizationsDialog.LocalizationBase), Value },
            { nameof(LocalizationsDialog.ReadOnly), Item.IsReadOnlyField(Field) },
            { nameof(LocalizationsDialog.Localizations), Localizations },
            { nameof(LocalizationsDialog.MaxLength), Field.MaxLength }
        };
        var result = await (await DialogService.ShowAsync<LocalizationsDialog>(
            Localizer.Item.EditTitle(Localizer.Shared.Localizations), parameters)).Result;
        if (result == null || result.Canceled)
        {
            return;
        }

        // localizations
        var localizations = result.Data as Dictionary<string, string>;
        if (localizations == null)
        {
            return;
        }
        await SetItemLocalizationsAsync(localizations);
    }

    private Dictionary<string, string> GetItemLocalizations()
    {
        if (!IsLocalizable)
        {
            return new();
        }

        var property = Item.GetType().GetLocalizationsProperty(Field.PropertyName);
        var localizations = property.GetValue(Item) as Dictionary<string, string>;
        if (localizations == null)
        {
            return new();
        }
        return new(localizations);
    }

    private async Task SetItemLocalizationsAsync(Dictionary<string, string> localizations)
    {
        if (!IsLocalizable)
        {
            throw new InvalidOperationException($"Property {Field.PropertyName} is not localizable");
        }

        var existingLocalizations = GetItemLocalizations();
        if (CompareTool.EqualDictionaries((IDictionary<string, string>)localizations, existingLocalizations))
        {
            return;
        }

        var property = Item.GetType().GetLocalizationsProperty(Field.PropertyName);
        property.SetValue(Item, localizations);

        // notifications
        UpdateState();
        await ValueChanged.InvokeAsync(localizations);
    }

    #endregion

    #region Lifecycle

    private void UpdateState()
    {
        var value = Value;

        // base value
        IsBaseValue = Field.HasBaseValues && !string.IsNullOrWhiteSpace(value) &&
                      string.Equals(value, GetBaseValue());

        StateHasChanged();
    }

    private IRegulationItem lastItem;

    protected override async Task OnInitializedAsync()
    {
        lastItem = Item;
        Localizations = GetItemLocalizations();
        ApplyFieldValue();
        UpdateState();
        await base.OnInitializedAsync();
    }

    protected override async Task OnParametersSetAsync()
    {
        if (lastItem != Item)
        {
            lastItem = Item;
            ApplyFieldValue();
            UpdateState();
        }
        await base.OnParametersSetAsync();
    }

    #endregion

}
