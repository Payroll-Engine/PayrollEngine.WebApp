using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;
using PayrollEngine.Client.Service;
using PayrollEngine.WebApp.ViewModel;
using Task = System.Threading.Tasks.Task;

namespace PayrollEngine.WebApp.Presentation.Regulation.Component;

public partial class CollectorMultiSelect : IRegulationInput
{
    [Parameter]
    public RegulationEditContext EditContext { get; set; }
    [Parameter]
    public IRegulationItem Item { get; set; }
    [Parameter]
    public RegulationField Field { get; set; }
    [Parameter]
    public bool ShowSelectAll { get; set; }
    [Parameter]
    public EventCallback<object> ValueChanged { get; set; }

    [Inject]
    private IPayrollService PayrollService { get; set; }

    protected List<RegulationCollector> Collectors { get; set; }
    protected List<string> Value { get; set; }

    protected List<string> FieldValue
    {
        get => Item.GetPropertyValue<List<string>>(Field.PropertyName);
        set => Item.SetPropertyValue(Field.PropertyName, value);
    }

    protected bool IsBaseValue { get; set; }

    public string CollectorsAsString
    {
        get
        {
            if (Collectors == null || !Collectors.Any())
            {
                return null;
            }
            return string.Join(',', Collectors.Select(x => x.Name));
        }
        set
        {
            Value ??= new();
            Value.Clear();
            if (value != null)
            {
                Value.AddRange(value.Split(',', StringSplitOptions.RemoveEmptyEntries));
            }
        }
    }

    private IEnumerable<string> SelectedCollectors { get; set; }

    #region Value

    private async Task SelectionChangedAsync(IEnumerable<string> value)
    {
        Value = value.ToList();
        await SetFieldValue(Value);
    }

    private async Task SetFieldValue(List<string> value)
    {
        // field value
        var fieldValue = value;
        if (fieldValue != null && fieldValue.Any())
        {
            var baseValue = GetBaseValue();
            if (baseValue != null &&
                !Field.Required && CompareTool.EqualDistinctLists(fieldValue, baseValue))
            {
                // reset value on non-mandatory fields
                fieldValue = null;
            }
        }
        FieldValue = fieldValue;
        ApplyFieldValue();

        // notifications
        UpdateState();
        await ValueChanged.InvokeAsync(value);
    }

    private void ApplyFieldValue()
    {
        // value
        var value = FieldValue;
        // base value
        if (value == null && Field.HasBaseValues)
        {
            value = GetBaseValue();
        }
        Value = value;

        // selection
        SelectedCollectors = Value;
    }

    protected List<string> GetBaseValue() =>
        Item.GetBaseValue<List<string>>(Field.PropertyName);

    #endregion

    #region Collectors

    /// <summary>
    /// Load collectors
    /// </summary>
    private async Task LoadCollectorsAsync()
    {
        Collectors = await PayrollService.GetCollectorsAsync<RegulationCollector>(
            new(EditContext.Tenant.Id, EditContext.Payroll.Id));
    }

    #endregion

    #region Lifecycle

    private void UpdateState()
    {
        var value = Value;

        // base value
        IsBaseValue = Field.HasBaseValues && value != null && value.Any() &&
                      CompareTool.EqualDistinctLists(value, GetBaseValue());

        StateHasChanged();
    }

    private IRegulationItem lastObject;

    protected override async Task OnInitializedAsync()
    {
        lastObject = Item;
        // The case request needs to be synchronously,
        // otherwise the rendering interrupts the sequence
        await Task.Run(LoadCollectorsAsync);
        ApplyFieldValue();
        UpdateState();
        await base.OnInitializedAsync();
    }

    protected override async Task OnParametersSetAsync()
    {
        if (lastObject != Item)
        {
            lastObject = Item;
            ApplyFieldValue();
            UpdateState();
        }
        await base.OnParametersSetAsync();
    }

    #endregion

}
