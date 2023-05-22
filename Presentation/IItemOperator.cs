using PayrollEngine.Client;
using Task = System.Threading.Tasks.Task;

namespace PayrollEngine.WebApp.Presentation;

public interface IItemOperator<in T>
    where T : IModel
{
    Task AddItemAsync();
    Task EditItemAsync(T item);
    Task DeleteItemAsync(T item);
}