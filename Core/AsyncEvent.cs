using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PayrollEngine.WebApp;

public class AsyncEvent<T>
{
    private readonly List<Func<object, T, Task>> items = new();
    private readonly object locker = new();

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

    public async Task InvokeAsync(object sender, T args)
    {
        List<Func<object, T, Task>> callbacks;
        lock (locker)
        {
            callbacks = new(items);
        }

        foreach (var callback in callbacks)
        {
            await callback(sender, args);
        }
    }
}