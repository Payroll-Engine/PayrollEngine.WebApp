using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using PayrollEngine.WebApp.Shared;
using Task = System.Threading.Tasks.Task;

namespace PayrollEngine.WebApp.Presentation.Component;

public partial class LocalizationsDialog
{
    [CascadingParameter] private MudDialogInstance MudDialog { get; set; }
    [Parameter]
    public string LocalizationBase { get; set; }
    [Parameter]
    public Dictionary<string, string> Localizations { get; set; } = new();
    [Parameter]
    public bool ReadOnly { get; set; }
    [Parameter]
    public int MaxLength { get; set; } = 524288;

    [Inject]
    private IDialogService DialogService { get; set; }
    [Inject]
    private Localizer Localizer { get; set; }

    protected MudDataGrid<KeyValuePair<string, string>> Grid { get; set; }

    protected async Task AddLocalizationAsync()
    {
        // readonly
        if (ReadOnly)
        {
            return;
        }

        // dialog parameters
        var parameters = new DialogParameters
        {
            { nameof(LocalizationDialog.LocalizationBase), LocalizationBase },
            { nameof(LocalizationDialog.Localizations), Localizations },
            { nameof(LocalizationDialog.MaxLength), MaxLength }
        };

        // localization create dialog
        var dialog = await (await DialogService.ShowAsync<LocalizationDialog>(
            Localizer.Item.AddTitle(Localizer.Localization.Localization), parameters)).Result;
        if (dialog == null || dialog.Canceled)
        {
            return;
        }

        // new localization
        if (!(dialog.Data is KeyValuePair<string, string> localization))
        {
            return;
        }

        // add/set localization
        Localizations[localization.Key] = localization.Value;
    }

    protected async Task EditLocalizationAsync(KeyValuePair<string, string> localization)
    {
        // readonly or existing
        if (ReadOnly || !Localizations.ContainsKey(localization.Key))
        {
            return;
        }

        // dialog parameters
        var parameters = new DialogParameters
        {
            { nameof(LocalizationDialog.LocalizationBase), LocalizationBase },
            { nameof(LocalizationDialog.Localizations), Localizations },
            { nameof(LocalizationDialog.Localization), localization }
        };

        // localization edit dialog
        var dialog = await (await DialogService.ShowAsync<LocalizationDialog>(
            Localizer.Item.EditTitle(Localizer.Localization.Localization), parameters)).Result;
        if (dialog == null || dialog.Canceled)
        {
            return;
        }

        // edited localization
        if (!(dialog.Data is KeyValuePair<string, string> editLocalization))
        {
            return;
        }

        // culture change
        if (!string.Equals(localization.Key, editLocalization.Key))
        {
            // remove old localization
            Localizations.Remove(localization.Key);
        }

        // apply localization
        Localizations[editLocalization.Key] = editLocalization.Value;
    }

    protected async Task DeleteLocalizationAsync(KeyValuePair<string, string> localization)
    {
        // readonly or existing
        if (ReadOnly || !Localizations.ContainsKey(localization.Key))
        {
            return;
        }

        // confirmation
        if (!await DialogService.ShowDeleteMessageBoxAsync(
                Localizer,
                Localizer.Item.DeleteTitle(Localizer.Localization.Localization),
                Localizer.Item.DeleteQuery(localization.Key)))
        {
            return;
        }

        // delete localization
        Localizations.Remove(localization.Key);
    }

    private void Cancel() => MudDialog.Cancel();

    private void Submit() => MudDialog.Close(DialogResult.Ok(Localizations));
}
