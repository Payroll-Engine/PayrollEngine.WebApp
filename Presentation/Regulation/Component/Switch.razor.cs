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

    private bool Value { get; set; }

    private bool FieldValue
    {
        get => Item.GetPropertyValue<bool>(Field.PropertyName);
        set => Item.SetPropertyValue(Field.PropertyName, value);
    }

    // ReSharper disable once UnusedMember.Local
    private async Task ValueChangedAsync(bool value) =>
        await SetFieldValue(value);

    // do not remove this dead method
    // method exists only prevent a ReSharper for the CheckedChanged event
    private async Task ValueChangedAsync(bool? value)
    {
        if (value.HasValue)
        {
            await SetFieldValue(value.Value);
        }
    }

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

    #endregion

    #region Lifecycle

    private void UpdateState() =>
        StateHasChanged();

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
