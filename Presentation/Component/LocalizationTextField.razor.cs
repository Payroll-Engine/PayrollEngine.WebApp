using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using PayrollEngine.Client;
using PayrollEngine.WebApp.Shared;
//using PayrollEngine.WebApp.Shared;
using Task = System.Threading.Tasks.Task;

namespace PayrollEngine.WebApp.Presentation.Component;

public partial class LocalizationTextField
{
    [Parameter]
    public IModel Item { get; set; }
    [Parameter]
    public string PropertyName { get; set; }
    [Parameter]
    public string Label { get; set; }
    [Parameter]
    public string HelperText { get; set; }
    [Parameter]
    public Variant Variant { get; set; }
    [Parameter]
    public string Value { get; set; }
    [Parameter]
    public EventCallback<string> ValueChanged { get; set; }
    [Parameter]
    public object Validation { get; set; }
    /// <summary>
    /// Read only text input
    /// </summary>
    [Parameter]
    public bool ReadOnly { get; set; }
    /// <summary>
    /// Read only localizations
    /// </summary>
    [Parameter]
    public bool ReadOnlyLocalization { get; set; }
    [Parameter]
    public bool Required { get; set; }
    [Parameter]
    public string RequiredError { get; set; }
    [Parameter]
    public int Lines { get; set; }
    [Parameter]
    public int MaxLength { get; set; } = 524288;

    [Inject]
    private IDialogService DialogService { get; set; }
    [Inject]
    private Localizer Localizer { get; set; }

    private Dictionary<string, string> Localizations { get; set; } = new();

    private Color LocalizationColor =>
        Localizations.Any() ? Color.Tertiary : Color.Primary;

    private async Task OpenLocalizationsAsync()
    {
        var parameters = new DialogParameters
        {
            { nameof(LocalizationsDialog.LocalizationBase), Value },
            { nameof(LocalizationsDialog.ReadOnly), ReadOnlyLocalization },
            { nameof(LocalizationsDialog.Localizations), Localizations }
        };
        var result = await (await DialogService.ShowAsync<LocalizationsDialog>(
            Localizer.Localization.DialogTitle(PropertyName.ToPascalSentence()), parameters)).Result;
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
        SetItemLocalizations(localizations);
    }

    private Dictionary<string, string> GetItemLocalizations()
    {
        var property = Item.GetType().GetLocalizationsProperty(PropertyName);
        var localizations = property.GetValue(Item) as Dictionary<string, string>;
        if (localizations == null)
        {
            return new();
        }
        return new(localizations);
    }

    private void SetItemLocalizations(Dictionary<string, string> localizations)
    {
        var existingLocalizations = GetItemLocalizations();
        if (CompareTool.EqualDictionaries((IDictionary<string, string>)localizations, existingLocalizations))
        {
            return;
        }

        var property = Item.GetType().GetLocalizationsProperty(PropertyName);
        property.SetValue(Item, localizations);
    }

    protected override async Task OnInitializedAsync()
    {
        Localizations = GetItemLocalizations();
        await base.OnInitializedAsync();
    }
}