using System;
using System.Linq;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Task = System.Threading.Tasks.Task;

namespace PayrollEngine.WebApp.Presentation.Component;

public class EnumFilterBase<T, TEnum> : ComponentBase
    where TEnum : struct, Enum
{
    [Parameter]
    public FilterContext<T> Context { get; set; }
    [Parameter]
    public MudDataGrid<T> DataGrid { get; set; }
    [Parameter]
    public string Column { get; set; }

    protected string FilterIcon { get; private set; } = Icons.Material.Outlined.FilterAlt;
    protected bool FilterOpen { get; private set; }

    private FilterDefinition<T> filterDefinition;

    #region Selection

    private TEnum selectedValue;
    private bool selection;

    protected void OpenFilter() =>
        FilterOpen = true;

    protected void CloseFilter() =>
        FilterOpen = false;

    protected async Task SelectFilterAsync(object value)
    {
        if (!(value is string stringValue))
        {
            return;
        }

        selection = Enum.GetNames<TEnum>().Any(x => Equals(x, stringValue));
        if (selection)
        {
            selectedValue = Enum.Parse<TEnum>(stringValue);
            filterDefinition.Value = selectedValue;
            await Context.Actions.ApplyFilterAsync(filterDefinition);
        }

        // update state
        FilterIcon = selection ? Icons.Material.Filled.FilterAlt : Icons.Material.Outlined.FilterAlt;
        FilterOpen = false;
    }

    protected async Task ClearFilter()
    {
        await Context.Actions.ClearFilterAsync(filterDefinition);

        // reset state
        selection = false;
        selectedValue = default;
        filterDefinition.Value = null;
        FilterIcon = Icons.Material.Outlined.FilterAlt;
    }

    #endregion

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            // filter column
            var column = DataGrid.RenderedColumns.FirstOrDefault(
                x => string.Equals(x.PropertyName, Column));
            if (column != null)
            {
                // filter
                filterDefinition = new()
                {
                    Column = column,
                    Operator = FilterOperator.Enum.Is,
                    Title = Column
                };
            }
        }
        await base.OnAfterRenderAsync(firstRender);
    }
}
