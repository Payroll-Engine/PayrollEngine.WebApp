using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using MudBlazor;

namespace PayrollEngine.WebApp.Presentation;

public class QueryBuilder<TQuery, TModel>
    where TQuery : Query, new()
    where TModel : class
{
    public int? MaximumItemCount { get; set; }

    public TQuery BuildQuery(GridState<TModel> state, IQueryResolver queryResolver = null)
    {
        var query = new TQuery
        {
            OrderBy = BuildSortString(state.SortDefinitions, queryResolver),
            Filter = BuildFilterString(state.FilterDefinitions, queryResolver)
        };

        // paging
        if (state.PageSize != 0)
        {
            // page size
            query.Top = state.PageSize;
            if (state.Page != 0)
            {
                // page offset
                query.Skip = state.Page * state.PageSize;
            }
        }
        else if (state.Page == 0 && MaximumItemCount != null)
        {
            // limit page size
            query.Top = MaximumItemCount.Value;
        }

        // count
        query.Result = QueryResultType.ItemsWithCount;
        return query;
    }

    #region Sort

    private string BuildSortString(ICollection<SortDefinition<TModel>> sorts, IQueryResolver queryResolver = null)
    {
        if (!sorts.Any())
        {
            return null;
        }

        var buffer = new StringBuilder();
        foreach (var sort in sorts)
        {
            // sort column separator
            if (sort != sorts.First())
            {
                buffer.Append(',');
            }

            var column = queryResolver != null ? queryResolver.GetSortColumn(sort.SortBy) : sort.SortBy;
            if (column != null)
            {
                buffer.Append($"{column} {(sort.Descending ? ODataParameters.Descending : ODataParameters.Ascending)}");
            }
        }
        return buffer.ToString();
    }

    #endregion

    #region Filter

    private string BuildFilterString(ICollection<IFilterDefinition<TModel>> filters, IQueryResolver queryResolver = null)
    {
        if (!filters.Any())
        {
            return null;
        }

        var filterString = string.Empty;
        // combined multiple where filters with AND
        foreach (var filter in filters)
        {
            var whereFilterString = FormatFilter(filter, queryResolver);
            if (string.IsNullOrWhiteSpace(whereFilterString))
            {
                continue;
            }

            filterString = string.IsNullOrWhiteSpace(filterString) ?
                // initial
                whereFilterString :
                // combined with AND
                $"{filterString} and {whereFilterString}";
        }
        return filterString;
    }

    private string FormatFilter(IFilterDefinition<TModel> whereFilter, IQueryResolver queryResolver = null)
    {
        // column
        var column = queryResolver != null ?
            queryResolver.GetColumnName(whereFilter.Column) :
            whereFilter.Column?.PropertyName;
        if (string.IsNullOrWhiteSpace(column))
        {
            return null;
        }

        // operator
        if (whereFilter.Operator == null)
        {
            return null;
        }

        var parameter = QueryParameters.Parameters.FirstOrDefault(x => x.Keys.Contains(whereFilter.Operator));
        if (parameter == null)
        {
            throw new ArgumentException($"Unknown filter operator {whereFilter.Operator}.");
        }

        // value
        var value = GetFilterValue(whereFilter);
        if (value == null && !parameter.OptionalValue)
        {
            return null;
        }

        // format
        var filter = string.Format(parameter.Template, column, value ?? "null");
        return filter;
    }

    private object GetFilterValue(IFilterDefinition<TModel> whereFilter)
    {
        if (whereFilter == null)
        {
            return null;
        }

        var value = whereFilter.Value;
        if (value == null)
        {
            return null;
        }

        // json value
        if (value is JsonElement jsonElement)
        {
            value = jsonElement.GetValue();
        }

        // date time
        if (value is string dateTimeString && DateTime.TryParse(dateTimeString, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out var dateTime))
        {
            value = dateTime;
        }
        if (value is DateTime dateTimeValue)
        {
            // ensure UTC date time
            if (!dateTimeValue.IsUtc())
            {
                dateTimeValue = dateTimeValue.ToUtc();
            }
            // OData compatible date time filter format (quoted)
            value = $"'{dateTimeValue.ToODataString()}'";
        }

        // string
        if (value is string stringValue)
        {
            if (string.IsNullOrWhiteSpace(stringValue))
            {
                // ensure db null
                value = null;
            }
            else
            {
                if (!stringValue.StartsWith("'"))
                {
                    stringValue = "'" + stringValue;
                }
                if (!stringValue.EndsWith("'"))
                {
                    stringValue += "'";
                }
                value = stringValue;
            }
        }
        else if (value.GetType().IsEnum)
        {
            // enum
            value = $"'{Enum.GetName(value.GetType(), value)}'";
        }

        return value;
    }

    #endregion

}