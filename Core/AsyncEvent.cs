using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace PayrollEngine.WebApp;

/// <summary>
/// Async events
/// </summary>
public class AsyncEvent<T>
{
    private readonly List<Func<object, T, Task>> items = [];
    private readonly object locker = new();

    /// <summary>
    /// + operator
    /// </summary>
    /// <param name="args">Event arguments</param>
    /// <param name="callback">Event callback</param>
    public static AsyncEvent<T> operator +(
        AsyncEvent<T> args, Func<object, T, Task> callback)
    {
        if (callback == null)
        {
            throw new ArgumentNullException(nameof(callback));
        }

        args ??= new();
        lock (args.locker)
        {
            args.items.Add(callback);
        }
        return args;
    }

    /// <summary>
    /// - operator
    /// </summary>
    /// <param name="args">Event arguments</param>
    /// <param name="callback">Event callback</param>
    public static AsyncEvent<T> operator -(
        AsyncEvent<T> args, Func<object, T, Task> callback)
    {
        if (callback == null)
        {
            throw new ArgumentNullException(nameof(callback));
        }
        if (args == null)
        {
            return null;
        }

        if (!args.items.Contains(callback))
        {
            Log.Trace($"Unknown async callback: {callback}", nameof(callback));
            return args;
        }

        lock (args.locker)
        {
            args.items.Remove(callback);
        }
        return args;
    }

    /// <summary>
    /// Invoke event
    /// </summary>
    /// <param name="sender">Event sender</param>
    /// <param name="args">Event arguments</param>
    public async Task InvokeAsync(object sender, T args)
    {
        List<Func<object, T, Task>> callbacks;
        lock (locker)
        {
            callbacks = [..items];
        }

        foreach (var callback in callbacks)
        {
            await callback(sender, args);
        }
    }
}