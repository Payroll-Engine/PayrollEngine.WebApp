using MudBlazor;

namespace PayrollEngine.WebApp.Presentation;

public static class GridStateExtensions
{
    public static Query ToQuery<TQuery, TItem>(this GridState<TItem> gridState,
        IQueryResolver resolver = null)
        where TItem : class, new()
        where TQuery : Query, new() =>
        new QueryBuilder<TQuery, TItem>().BuildQuery(gridState, resolver);
}