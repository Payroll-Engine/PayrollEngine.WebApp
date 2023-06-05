using System;
using System.IO;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using PayrollEngine.WebApp.ViewModel;
using Task = System.Threading.Tasks.Task;

namespace PayrollEngine.WebApp.Presentation.Regulation.Component
{
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
        /// <summary>Override field help</summary>
        [Parameter]
        public string HelperText { get; set; }

        [Inject]
        private IUserNotificationService UserNotification { get; set; }
        [Inject]
        private IDialogService DialogService { get; set; }

        private bool ClearDisabled() =>
            Item.IsReadOnlyField(Field) || string.IsNullOrWhiteSpace(Value);
        private bool UploadDisabled() => Item.IsReadOnlyField(Field);

        #region Upload

        private string ContentInfo { get; set; }

        private void UpdateInfo()
        {
            ContentInfo = string.IsNullOrWhiteSpace(Value) ?
                "None" :
                $"Size {Value.Length}";
        }

        private async Task UploadAsync(IBrowserFile file)
        {
            // confirmation
            if (!string.IsNullOrWhiteSpace(Value) &&
                !await DialogService.ShowMessageBoxAsync(
                    "Content Upload",
                    "Replace content?",
                    yesText: "Replace",
                    cancelText: "Cancel"))
            {
                return;
            }

            try
            {
                await using var stream = file.OpenReadStream();
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
                    await UserNotification.ShowSuccessAsync($"Document {file.Name} uploaded");
                }
                else
                {
                    await UserNotification.ShowWarningAsync($"Document {file.Name} unchanged");
                }
            }
            catch (Exception exception)
            {
                Log.Error(exception, exception.GetBaseMessage());
                await UserNotification.ShowErrorMessageBoxAsync("File upload error", exception);
            }
        }

        #endregion

        #region Value

        protected string Value { get; set; }

        protected string FieldValue
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
                    "Clear Content",
                    new MarkupString($"Delete content (length {Value.Length})?")))
            {
                return;
            }

            await SetFieldValue(null);

            // notifications
            await ValueChangedAsync(null);
            UpdateState();
            await UserNotification.ShowSuccessAsync("Document removed");
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
}
