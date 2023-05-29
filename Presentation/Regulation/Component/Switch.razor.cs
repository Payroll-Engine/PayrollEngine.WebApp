using Microsoft.AspNetCore.Components;
using MudBlazor;
using PayrollEngine.WebApp.ViewModel;
using Task = System.Threading.Tasks.Task;

namespace PayrollEngine.WebApp.Presentation.Regulation.Component;

public partial class Switch : IRegulationInput
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
    public Color Color { get; set; } = Color.Primary;

    #region Value

    protected bool Value { get; set; }

    protected bool FieldValue
    {
        get => Item.GetPropertyValue<bool>(Field.PropertyName);
        set => Item.SetPropertyValue(Field.PropertyName, value);
    }

    protected bool IsBaseValue { get; set; }

    private async Task ValueChangedAsync(bool value) =>
        await SetFieldValue(value);

    private async Task SetFieldValue(bool value)
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

    #endregion

    #region Lifecycle

    private void UpdateState()
    {
        var value = Value;

        // base value
        IsBaseValue = Field.HasBaseValues && Equals(value, GetBaseValue());
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
