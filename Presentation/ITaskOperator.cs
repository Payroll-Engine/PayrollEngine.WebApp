using PayrollEngine.Client.Model;
using Task = System.Threading.Tasks.Task;

namespace PayrollEngine.WebApp.Presentation;

public interface ITaskOperator<in T> : IItemOperator<T>
    where T : ITask
{
    Task CompleteItemAsync(T item);
}