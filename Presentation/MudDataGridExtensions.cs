using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MudBlazor;

namespace PayrollEngine.WebApp.Presentation;

/// <summary>
/// Extension methods for <see cref="MudDataGrid{T}" />
/// </summary>
public static class MudDataGridExtensions
{
    /// <param name="dataGrid">Data grid</param>
    extension<T>(MudDataGrid<T> dataGrid)
    {
        /// <summary>
        /// Build the grid export state
        /// </summary>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Page size</param>
        public GridState<T> BuildExportState(int page = 0,
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

        /// <summary>
        /// Set column filter
        /// </summary>
        /// <param name="columnName">Column name</param>
        /// <param name="filterOperator">Filter operator</param>
        /// <param name="value">Filter value</param>
        /// <param name="title">Filter title</param>
        public async Task SetColumnFilterAsync(string columnName, string filterOperator, object value, string title = null)
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

        /// <summary>
        /// Get column properties
        /// </summary>
        public List<string> GetColumnProperties()
        {
            var properties = new List<string>();
            foreach (var column in dataGrid.RenderedColumns)
            {
                if (column.Tag is string attributeTag)
                {
                    var attributeName = attributeTag.RemoveAttributePrefix();
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
}