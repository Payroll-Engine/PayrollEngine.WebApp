using Microsoft.AspNetCore.Components;
using MudBlazor;
using PayrollEngine.Client.Model;
using PayrollEngine.WebApp.ViewModel;
using Task = System.Threading.Tasks.Task;

namespace PayrollEngine.WebApp.Presentation.Regulation.Component
{
    public partial class LookupSettingsBox : IRegulationInput
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
            Item.IsReadOnlyField(Field) || Value == null ||
            string.IsNullOrWhiteSpace(Value.LookupName);
        private bool EditDisabled() =>
            Item.IsReadOnlyField(Field);

        private string SettingsInfo { get; set; }

        #region Value

        protected LookupSettings Value { get; set; }

        protected LookupSettings FieldValue
        {
            get => Item.GetPropertyValue<LookupSettings>(Field.PropertyName);
            set => Item.SetPropertyValue(Field.PropertyName, value);
        }

        private async Task ValueChangedAsync(LookupSettings value) =>
            await SetFieldValue(value);

        private async Task SetFieldValue(LookupSettings value)
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

            // info
            UpdateInfo();
        }

        protected bool GetBaseValue() =>
            Item.GetBaseValue<bool>(Field.PropertyName);

        private async Task ClearSettingsAsync()
        {
            if (Value == null)
            {
                return;
            }

            // confirmation
            if (!await DialogService.ShowDeleteMessageBoxAsync(
                    "Clear settings",
                    "Delete lookup settings?"))
            {
                return;
            }

            await SetFieldValue(null);

            // notifications
            await ValueChangedAsync(null);
            UpdateState();
            await UserNotification.ShowSuccessAsync("Lookup settings removed");
        }

        private async Task EditSettingsAsync()
        {
            Value ??= new();

            // edit copy
            var editItem = new LookupSettings(Value);

            // dialog parameters
            var parameters = new DialogParameters
            {
                { nameof(LookupSettingsDialog.Tenant), EditContext.Tenant },
                { nameof(LookupSettingsDialog.Payroll), EditContext.Payroll },
                { nameof(LookupSettingsDialog.Settings), editItem },
                { nameof(LookupSettingsDialog.Language), EditContext.User.Language }
            };

            // attribute edit dialog
            var dialog = await (await DialogService.ShowAsync<LookupSettingsDialog>("Edit lookup settings", parameters)).Result;
            if (dialog == null || dialog.Canceled)
            {
                return;
            }

            await SetFieldValue(editItem);
        }

        #endregion

        #region Lifecycle

        private void UpdateInfo()
        {
            var info = Value == null || string.IsNullOrWhiteSpace(Value.LookupName) ?
                "None" : $"Lookup {Value.LookupName}";
            if (Value != null && !string.IsNullOrWhiteSpace(Value.ValueFieldName))
            {
                info += $" - Value {Value.ValueFieldName}";
            }
            if (Value != null && !string.IsNullOrWhiteSpace(Value.TextFieldName))
            {
                info += $" - Text {Value.TextFieldName}";
            }
            SettingsInfo = info;
        }

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
