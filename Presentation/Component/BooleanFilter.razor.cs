using System.Linq;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using PayrollEngine.WebApp.Shared;
using Task = System.Threading.Tasks.Task;

namespace PayrollEngine.WebApp.Presentation.Component;

public class BooleanFilterBase<T> : ComponentBase
{
    [Parameter]
    public FilterContext<T> Context { get; set; }
    [Parameter]
    public MudDataGrid<T> DataGrid { get; set; }
    [Parameter]
    public string Column { get; set; }

    [Inject]
    protected Localizer Localizer { get; set; }

    protected string FilterIcon { get; private set; } = Icons.Material.Outlined.FilterAlt;
    protected bool FilterOpen { get; private set; }

    private FilterDefinition<T> filterDefinition;

    #region Selection

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

        if (bool.TryParse(stringValue, out var boolValue))
        {
            // use int to filter boolean values
            filterDefinition.Value = boolValue ? 1 : 0;
            await Context.Actions.ApplyFilterAsync(filterDefinition);
            FilterIcon = Icons.Material.Filled.FilterAlt;
        }
        else
        {
            FilterIcon = Icons.Material.Outlined.FilterAlt;
        }

        // update state
        FilterOpen = false;
    }

    protected async Task ClearFilter()
    {
        await Context.Actions.ClearFilterAsync(filterDefinition);

        // reset state
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
                    Operator = FilterOperator.Boolean.Is,
                    Title = Column
                };
            }
        }
        await base.OnAfterRenderAsync(firstRender);
    }
}
