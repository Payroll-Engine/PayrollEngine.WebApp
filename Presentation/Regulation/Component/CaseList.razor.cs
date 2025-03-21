﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;
using PayrollEngine.Client.Service;
using PayrollEngine.WebApp.ViewModel;
using Task = System.Threading.Tasks.Task;

namespace PayrollEngine.WebApp.Presentation.Regulation.Component;

public partial class CaseList : IRegulationInput
{
    [Parameter]
    public RegulationEditContext EditContext { get; set; }
    [Parameter]
    public IRegulationItem Item { get; set; }
    [Parameter]
    public RegulationField Field { get; set; }
    [Parameter]
    public EventCallback<object> ValueChanged { get; set; }

    [Inject]
    private IPayrollService PayrollService { get; set; }
    [Inject]
    private IUserNotificationService UserNotification { get; set; }

    private List<RegulationCase> Cases { get; set; } = [];
    private RegulationCase SelectedCase { get; set; }
    private string Value { get; set; }

    public bool AllowClear => !Field.KeyField && !Field.Required;

    private string FieldValue
    {
        get => Item.GetPropertyValue<string>(Field.PropertyName);
        set => Item.SetPropertyValue(Field.PropertyName, value);
    }

    #region Value

    private async Task ValueChangedAsync(RegulationCase @case) =>
        await SetFieldValue(@case.Name);

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

    #region Cases

    /// <summary>
    /// Load cases
    /// <remarks>The case request needs to be synchronously,
    /// otherwise the rendering interrupts the sequence</remarks>
    /// </summary>
    private async Task LoadCasesAsync()
    {
        var cases = await PayrollService.GetCasesAsync<RegulationCase>(
            new(EditContext.Tenant.Id, EditContext.Payroll.Id));
        Cases = cases.OrderBy(x => x.Name).ToList();
    }

    #endregion

    #region Lifecycle

    private async Task UpdateStateAsync()
    {
        var value = Value;

        // selected case
        RegulationCase selectedCase = null;
        if (!string.IsNullOrWhiteSpace(value))
        {
            selectedCase = Cases.FirstOrDefault(x => string.Equals(x.Name, value));
            if (selectedCase == null)
            {
                await UserNotification.ShowErrorAsync($"Unknown case {value}");
            }
        }
        SelectedCase = selectedCase;

        StateHasChanged();
    }

    private IRegulationItem lastItem;
    protected override async Task OnInitializedAsync()
    {
        lastItem = Item;
        // The case request needs to be synchronously,
        // otherwise the rendering interrupts the sequence
        await LoadCasesAsync();
        ApplyFieldValue();
        await UpdateStateAsync();
        await base.OnInitializedAsync();
    }

    protected override async Task OnParametersSetAsync()
    {
        if (lastItem != Item)
        {
            lastItem = Item;
            ApplyFieldValue();
            await UpdateStateAsync();
        }
        await base.OnParametersSetAsync();
    }

    #endregion

}
