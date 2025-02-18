using PayrollEngine.Client;
using Task = System.Threading.Tasks.Task;

namespace PayrollEngine.WebApp.Presentation;

/// <summary>
/// Item operator
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IItemOperator<in T>
    where T : IModel
{
    /// <summary>
    /// Create item
    /// </summary>
    Task CreateItemAsync();

    /// <summary>
    /// Edit item
    /// </summary>
    /// <param name="item">Item to edit</param>
    Task EditItemAsync(T item);

    /// <summary>
    /// Delete item
    /// </summary>
    /// <param name="item">Item to delete</param>
    Task DeleteItemAsync(T item);
}