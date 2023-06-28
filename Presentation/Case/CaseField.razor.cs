using System.Linq;
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
    public Language Language { get; set; }
    [Parameter]
    public bool Dense { get; set; }
    [Parameter]
    public Variant Variant { get; set; }

    [Inject]
    private IDialogService DialogService { get; set; }
    [Inject]
    private Localizer Localizer { get; set; }

    #region Change history

    private bool HistoryValuesAvailable =>
        Field.HistoryValues != null && Field.HistoryValues.Any();

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
        var accept = Field.Attributes?.GetAttachmentExtensions(Language);
        var parameters = new DialogParameters
        {
            { nameof(CaseDocumentsDialog<CaseDocument>.Documents), caseField.Documents },
            { nameof(CaseDocumentsDialog<CaseDocument>.Accept), accept },
            { nameof(CaseDocumentsDialog<CaseDocument>.Editable), true }
        };

        // dialog
        var dialog = await (await DialogService.ShowAsync<CaseDocumentsDialog<CaseDocument>>(
            "Case field documents", parameters)).Result;
        if (dialog == null || dialog.Canceled)
        {
            return;
        }
        StateHasChanged();
    }

    #endregion

}