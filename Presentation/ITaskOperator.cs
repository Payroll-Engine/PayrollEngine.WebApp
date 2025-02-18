using PayrollEngine.Client.Model;
using Task = System.Threading.Tasks.Task;

namespace PayrollEngine.WebApp.Presentation;

/// <summary>
/// Task operator
/// </summary>
/// <typeparam name="T"></typeparam>
public interface ITaskOperator<in T> : IItemOperator<T>
    where T : ITask
{
    /// <summary>
    /// Complete item
    /// </summary>
    /// <param name="item">Item to complete</param>
    Task CompleteItemAsync(T item);
}