using Microsoft.AspNetCore.Components;
using PayrollEngine.WebApp.ViewModel;
using Task = System.Threading.Tasks.Task;

namespace PayrollEngine.WebApp.Presentation.Regulation.Component;

public partial class NumericTextBox<T> : ComponentBase, IRegulationInput
{
    [Parameter]
    public RegulationEditContext EditContext { get; set; }
    [Parameter]
    public IRegulationItem Item { get; set; }
    [Parameter]
    public RegulationField Field { get; set; }
    [Parameter]
    public EventCallback<object> ValueChanged { get; set; }

   // protected bool IsBaseValue { get; set; }

    #region Value

    private T Value { get; set; }

    private T FieldValue
    {
        get => Item.GetPropertyValue<T>(Field.PropertyName);
        set => Item.SetPropertyValue(Field.PropertyName, value);
    }

    private async Task ValueChangedAsync(T item) =>
        await SetFieldValue(item);

    private async Task SetFieldValue(T value)
    {
        // field value
        if (Item.IsNew() || !Field.KeyField)
        {
            FieldValue = value;
            ApplyFieldValue();
        }

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
    }

    private T GetBaseValue() =>
        Item.GetBaseValue<T>(Field.PropertyName);

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
