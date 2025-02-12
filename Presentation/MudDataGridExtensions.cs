using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MudBlazor;

namespace PayrollEngine.WebApp.Presentation;

public static class MudDataGridExtensions
{
    public static GridState<T> BuildExportState<T>(this MudDataGrid<T> dataGrid, int page = 0,
        int pageSize = 0)
    {
        // server request
        var state = new GridState<T>
        {
            Page = page,
            PageSize = pageSize
        };

        // grid filter
        foreach (var filterDefinition in dataGrid.FilterDefinitions)
        {
            state.FilterDefinitions.Add(filterDefinition);
        }

        // grid sort
        foreach (var sortDefinition in dataGrid.SortDefinitions)
        {
            state.SortDefinitions.Add(sortDefinition.Value);
        }

        return state;
    }

    public static async Task SetColumnFilterAsync<T>(this MudDataGrid<T> dataGrid,
        string columnName, string filterOperator, object value, string title = null)
    {
        if (string.IsNullOrWhiteSpace(columnName))
        {
            throw new ArgumentException(nameof(columnName));
        }

        if (filterOperator == null)
        {
            throw new ArgumentNullException(nameof(filterOperator));
        }

        // completed tasks filter
        var column = dataGrid.RenderedColumns.FirstOrDefault(
            x => string.Equals(x.PropertyName, columnName));
        if (column == null)
        {
            return;
        }

        FilterDefinition<T> filter = new()
        {
            Column = column,
            Operator = filterOperator,
            Value = value,
            Title = title
        };

        await dataGrid.ClearFiltersAsync();
        await dataGrid.AddFilterAsync(filter);
    }

    public static List<string> GetColumnProperties<T>(this MudDataGrid<T> dataGrid)
    {
        var properties = new List<string>();
        foreach (var column in dataGrid.RenderedColumns)
        {
            if (column.Tag is string attributeTag)
            {
                string attributeName = attributeTag.RemoveAttributePrefix();
                if (string.IsNullOrWhiteSpace(attributeName))
                {
                    throw new PayrollException($"Invalid attribute tag {attributeTag} in grid column {column.Title}.");
                }
                properties.Add($"{nameof(IAttributeObject.Attributes)}.{attributeName}");
            }
            else
            {
                properties.Add(column.PropertyName);
            }
        }
        return properties;
    }
}