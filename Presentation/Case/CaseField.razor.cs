using System.Linq;
using System.Globalization;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using PayrollEngine.Client.Model;
using PayrollEngine.WebApp.Shared;
using CaseFieldSet = PayrollEngine.WebApp.ViewModel.CaseFieldSet;
using Task = System.Threading.Tasks.Task;

namespace PayrollEngine.WebApp.Presentation.Case;

public partial class CaseField
{
    [Parameter]
    public CaseFieldSet Field { get; set; }
    [Parameter]
    public CultureInfo Culture { get; set; }
    [Parameter]
    public bool Dense { get; set; }
    [Parameter]
    public Variant Variant { get; set; }

    [Inject]
    private IDialogService DialogService { get; set; }
    [Inject]
    private ILocalizerService LocalizerService { get; set; }

    private Localizer Localizer => LocalizerService.Localizer;

    #region Change History

    private bool UseChangeHistory { get; set; } = true;
    private bool VisibleChangeHistory => !ChangeHistoryLoaded || ChangeHistoryAvailable;
    private bool ChangeHistoryLoaded { get; set; }
    private bool ChangeHistoryAvailable { get; set; }

    private void InitChangeHistory()
    {
        var userHistory = Field.Attributes.GetValueHistory(Culture);
        UseChangeHistory = userHistory ?? true;
    }

    private async Task ViewChangeHistoryAsync()
    {
        // load history values
        var history = await Field.LoadHistoryValuesAsync();
        ChangeHistoryLoaded = true;
        ChangeHistoryAvailable = history.Any();

        // no history
        if (!ChangeHistoryLoaded)
        {
            await UserNotificationService.ShowMessageBoxAsync(
                Localizer,
                Localizer.CaseField.CaseField,
                Localizer.CaseField.EmptyChangeHistory);
            return;
        }

        // history dialog
        var parameters = new DialogParameters
        {
            { nameof(ChangeHistoryDialog.History), history },
            { nameof(ChangeHistoryDialog.Field), Field },
            { nameof(ChangeHistoryDialog.Culture), Culture }
        };
        await DialogService.ShowAsync<ChangeHistoryDialog>(Localizer.CaseField.ChangeHistory, parameters);
    }

    #endregion

    #region Attachments

    private bool AttachmentsSupported =>
        Field.AttachmentType != AttachmentType.None;

    private bool AttachmentsValid
    {
        get
        {
            switch (Field.AttachmentType)
            {
                case AttachmentType.Optional:
                    return true;
                case AttachmentType.Mandatory:
                    return Field.Validator.ValidateAttachment();
                case AttachmentType.None:
                default:
                    return false;
            }
        }
    }

    private bool AttachmentsAvailable =>
        AttachmentsSupported && Field.Documents.Any();

    private async Task EditAttachmentsAsync(CaseFieldSet caseField)
    {
        // editable document dialog
        var accept = Field.Attributes?.GetAttachmentExtensions(Culture);
        var parameters = new DialogParameters
        {
            { nameof(CaseDocumentsDialog<CaseDocument>.Documents), caseField.Documents },
            { nameof(CaseDocumentsDialog<CaseDocument>.Accept), accept },
            { nameof(CaseDocumentsDialog<CaseDocument>.Editable), true }
        };

        // dialog
        var dialog = await (await DialogService.ShowAsync<CaseDocumentsDialog<CaseDocument>>(
            Localizer.CaseField.CaseFieldDocuments, parameters)).Result;
        if (dialog == null || dialog.Canceled)
        {
            return;
        }
        StateHasChanged();
    }

    #endregion

    #region Lifecycle

    protected override void OnInitialized()
    {
        InitChangeHistory();
        base.OnInitialized();
    }

    #endregion

}