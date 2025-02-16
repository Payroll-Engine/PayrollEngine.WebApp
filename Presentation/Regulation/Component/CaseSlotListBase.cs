using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;
using PayrollEngine.Client.Model;
using PayrollEngine.Client.Service;
using PayrollEngine.WebApp.Shared;
using PayrollEngine.WebApp.ViewModel;
using Task = System.Threading.Tasks.Task;

namespace PayrollEngine.WebApp.Presentation.Regulation.Component;

public abstract class CaseSlotListBase : ComponentBase, IRegulationInput
{
    [Parameter] public RegulationEditContext EditContext { get; set; }
    [Parameter] public IRegulationItem Item { get; set; }
    [Parameter] public RegulationField Field { get; set; }
    [Parameter] public EventCallback<object> ValueChanged { get; set; }

    [Inject] protected ILocalizerService LocalizerService { get; set; }
    [Inject] private IPayrollService PayrollService { get; set; }
    [Inject] private IUserNotificationService UserNotification { get; set; }

    protected List<CaseSlot> CaseSlots { get; private set; } = [];
    protected CaseSlot SelectedCaseSlot { get; private set; }

    protected Localizer Localizer => LocalizerService.Localizer;
    private string Value { get; set; }

    public bool AllowClear => !Field.KeyField && !Field.Required;

    private string FieldValue
    {
        get => Item.GetPropertyValue<string>(Field.PropertyName);
        set => Item.SetPropertyValue(Field.PropertyName, value);
    }

    #region Value

    protected async Task ValueChangedAsync(CaseSlot caseSlot) =>
        await SetFieldValue(caseSlot.Name);

    private async Task SetFieldValue(string value)
    {
        // field value
        var fieldValue = value;
        if (fieldValue != null && fieldValue.Any())
        {
            var baseValue = GetBaseValue();
            if (baseValue != null &&
                !Field.Required && Equals(fieldValue, baseValue))
            {
                // reset value on non-mandatory fields
                fieldValue = null;
            }
        }

        FieldValue = fieldValue;
        ApplyFieldValue();

        // notifications
        await UpdateStateAsync();
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

    private string GetBaseValue() =>
        Item.GetBaseValue<string>(Field.PropertyName);

    #endregion

    #region Case slots

    protected abstract string GetSlotCaseName();

    /// <summary>
    /// Setup case slots
    /// </summary>
    private async Task SetupCaseSlotsAsync()
    {
        var caseSlots = new List<CaseSlot>();

        var @case = (await PayrollService.GetCasesAsync<RegulationCase>(
            new(EditContext.Tenant.Id, EditContext.Payroll.Id),
            caseNames: [GetSlotCaseName()])).FirstOrDefault();
        if (@case?.Slots != null)
        {
            foreach (var slot in @case.Slots)
            {
                var existing = caseSlots.FirstOrDefault(x => string.Equals(x.Name, slot.Name));
                if (existing == null)
                {
                    caseSlots.Add(slot);
                }
            }
        }

        CaseSlots = caseSlots;
    }

    #endregion

    #region Lifecycle

    private async Task UpdateStateAsync()
    {
        var value = Value;

        // selected slot
        CaseSlot selectedCaseSlot = null;
        if (!string.IsNullOrWhiteSpace(value))
        {
            selectedCaseSlot = CaseSlots.FirstOrDefault(x => string.Equals(x.Name, value));
            if (selectedCaseSlot == null)
            {
                await UserNotification.ShowErrorAsync($"Unknown case slot {value}");
            }
        }

        SelectedCaseSlot = selectedCaseSlot;

        StateHasChanged();
    }

    private IRegulationItem lastItem;

    protected override async Task OnInitializedAsync()
    {
        lastItem = Item;
        // The case request needs to be synchronously,
        // otherwise the rendering interrupts the sequence
        await SetupCaseSlotsAsync();
        ApplyFieldValue();
        await UpdateStateAsync();
        await base.OnInitializedAsync();
    }

    protected override async Task OnParametersSetAsync()
    {
        if (lastItem != Item)
        {
            lastItem = Item;
            await SetupCaseSlotsAsync();
            ApplyFieldValue();
            await UpdateStateAsync();
        }

        await base.OnParametersSetAsync();
    }

    #endregion

}