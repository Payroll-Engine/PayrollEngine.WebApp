using MudBlazor;

namespace PayrollEngine.WebApp.Presentation;

/// <summary>
/// Query resolve tool
/// </summary>
public interface IQueryResolver
{
    /// <summary>
    /// Get sort column name from expression
    /// </summary>
    /// <param name="expression">The expression</param>
    /// <returns>The column name</returns>
    public string GetSortColumn(string expression);

    /// <summary>
    /// Get typed column name
    /// </summary>
    /// <typeparam name="T">The column type</typeparam>
    /// <param name="column">The column</param>
    /// <returns>The column name</returns>
    public string GetColumnName<T>(Column<T> column) where T : class;
}