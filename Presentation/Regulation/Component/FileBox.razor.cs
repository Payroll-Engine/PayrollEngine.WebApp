using System;
using System.IO;
using Task = System.Threading.Tasks.Task;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using PayrollEngine.WebApp.Shared;
using PayrollEngine.WebApp.ViewModel;

namespace PayrollEngine.WebApp.Presentation.Regulation.Component;

public partial class FileBox : IRegulationInput
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
    [Parameter]
    public string HelperText { get; set; }

    [Inject]
    private IUserNotificationService UserNotification { get; set; }
    [Inject] 
    private IDownloadService DownloadService { get; set; }
    [Inject]
    private IDialogService DialogService { get; set; }
    [Inject]
    private Localizer Localizer { get; set; }

    private bool ClearDisabled() =>
        Item.IsReadOnlyField(Field) || string.IsNullOrWhiteSpace(Value);
    private bool UploadDisabled() => Item.IsReadOnlyField(Field);

    private string BorderStyle =>
        Field.Required && string.IsNullOrWhiteSpace(Value) ? "border-color: red" : string.Empty;

    #region Upload

    private string ContentInfo { get; set; }

    private void UpdateInfo()
    {
        ContentInfo = string.IsNullOrWhiteSpace(Value) ?
            Localizer.Shared.None :
            Localizer.Document.FileSize(Value.Length);
    }

    private async Task UploadAsync(IBrowserFile file)
    {
        // confirmation
        if (!string.IsNullOrWhiteSpace(Value) &&
            !await DialogService.ShowMessageBoxAsync(
                Localizer.Document.ContentUpload,
                Localizer.Document.ContentReplaceQuery,
                yesText: Localizer.Document.Replace,
                cancelText: Localizer.Dialog.Cancel))
        {
            return;
        }

        try
        {
            await using var stream = file.OpenReadStream(DownloadService.MaxAllowedSize);
            using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);

            // content change
            var value = Convert.ToBase64String(memoryStream.ToArray());
            if (!string.Equals(Value, value))
            {
                Value = value;

                // notifications
                await ValueChangedAsync(value);
                UpdateState();
                await UserNotification.ShowSuccessAsync(Localizer.Document.ContentUploaded(file.Name));
            }
            else
            {
                await UserNotification.ShowWarningAsync(Localizer.Document.ContentUnchanged(file.Name));
            }
        }
        catch (Exception exception)
        {
            Log.Error(exception, exception.GetBaseMessage());
            await UserNotification.ShowErrorMessageBoxAsync(Localizer, Localizer.Error.FileUploadError, exception);
        }
    }

    #endregion

    #region Value

    private string Value { get; set; }

    private string FieldValue
    {
        get => Item.GetPropertyValue<string>(Field.PropertyName);
        set => Item.SetPropertyValue(Field.PropertyName, value);
    }

    private async Task ValueChangedAsync(string value) =>
        await SetFieldValue(value);

    private async Task SetFieldValue(string value)
    {
        FieldValue = value;
        ApplyFieldValue();

        // notifications
        UpdateState();
        await ValueChanged.InvokeAsync(value);
    }

    private void ApplyFieldValue()
    {
        // value
        Value = FieldValue;
    }

    protected bool GetBaseValue() =>
        Item.GetBaseValue<bool>(Field.PropertyName);

    private async Task ClearContentAsync()
    {
        if (string.IsNullOrWhiteSpace(Value))
        {
            return;
        }

        // confirmation
        if (!await DialogService.ShowDeleteMessageBoxAsync(
                Localizer,
                Localizer.Document.ClearContent,
                Localizer.Document.ClearContentQuery(Value.Length)))
        {
            return;
        }

        await SetFieldValue(null);

        // notifications
        await ValueChangedAsync(null);
        UpdateState();
    }

    #endregion

    #region Lifecycle

    private void UpdateState()
    {
        UpdateInfo();
        StateHasChanged();
    }

    private IRegulationItem lastItem;

    protected override async Task OnInitializedAsync()
    {
        lastItem = Item;
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