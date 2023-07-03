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
    private TEnum selectedValue;
    private bool selection;

    protected void OpenFilter()
    {
        FilterOpen = true;
    }

    protected void CloseFilter()
    {
        FilterOpen = false;
    }

    protected async Task SelectFilter(object value)
    {
        var enumValues = Enum.GetValues<TEnum>();
        selection = enumValues.Any(x => Equals(x, value));
        if (selection)
        {
            selectedValue = enumValues.First(x => Equals(x, value));
            filterDefinition.Value = selectedValue;
            await Context.Actions.ApplyFilterAsync(filterDefinition);
        }

        // update state
        FilterIcon = selection ? Icons.Material.Outlined.FilterAlt : Icons.Material.Filled.FilterAlt;
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

    protected override Task OnAfterRenderAsync(bool firstRender)
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
                    Operator = "equals",
                    Title = Column
                };
            }
        }
        return base.OnAfterRenderAsync(firstRender);
    }
}
