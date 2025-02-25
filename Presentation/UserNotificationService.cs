using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using PayrollEngine.WebApp.Shared;

namespace PayrollEngine.WebApp.Presentation;

/// <inheritdoc />
public class UserNotificationService : IUserNotificationService
{
    private IUserNotificationService handler;

    /// <summary>
    /// Initialize user notification
    /// </summary>
    /// <param name="notificationHandler">Notification handler</param>
    public void Initialize(IUserNotificationService notificationHandler)
    {
        if (IsInitialized)
        {
            throw new InvalidOperationException("User notification service already initialized");
        }
        handler = notificationHandler;
    }

    /// <inheritdoc />
    public bool IsInitialized => handler != null;

    /// <inheritdoc />
    public async Task ShowAsync(string message)
    {
        if (handler == null)
        {
            return;
        }
        await handler.ShowAsync(message);
    }

    /// <inheritdoc />
    public async Task ShowInformationAsync(string message)
    {
        if (handler == null)
        {
            return;
        }
        await handler.ShowInformationAsync(message);
    }

    /// <inheritdoc />
    public async Task ShowWarningAsync(string message)
    {
        if (handler == null)
        {
            return;
        }
        await handler.ShowWarningAsync(message);
    }

    /// <inheritdoc />
    public async Task ShowSuccessAsync(string message)
    {
        if (handler == null)
        {
            return;
        }
        await handler.ShowSuccessAsync(message);
    }

    /// <inheritdoc />
    public async Task ShowErrorAsync(string message)
    {
        if (handler == null)
        {
            return;
        }
        await handler.ShowErrorAsync(message);
    }

    /// <inheritdoc />
    public async Task ShowErrorAsync(Exception exception, string message = null)
    {
        if (handler == null)
        {
            return;
        }
        await handler.ShowErrorAsync(exception, message);
    }

    /// <inheritdoc />
    public async Task<bool> ShowMessageBoxAsync(Localizer localizer, string title, string message,
        string yesText = null, string noText = null, string cancelText = null, string icon = null)
    {
        if (handler == null)
        {
            return false;
        }
        return await handler.ShowMessageBoxAsync(localizer, title, message, yesText, noText, cancelText, icon);
    }

    /// <inheritdoc />
    public async Task<bool> ShowMessageBoxAsync(Localizer localizer, string title, MarkupString message,
        string yesText = null, string noText = null, string cancelText = null, string icon = null)
    {
        if (handler == null)
        {
            return false;
        }
        return await handler.ShowMessageBoxAsync(localizer, title, message, yesText, noText, cancelText, icon);
    }

    /// <inheritdoc />
    public async Task<bool> ShowDeleteMessageBoxAsync(Localizer localizer, string title, string message)
    {
        if (handler == null)
        {
            return false;
        }
        return await handler.ShowDeleteMessageBoxAsync(localizer, title, message);
    }

    /// <inheritdoc />
    public async Task<bool> ShowDeleteMessageBoxAsync(Localizer localizer, string title, MarkupString message)
    {
        if (handler == null)
        {
            return false;
        }
        return await handler.ShowDeleteMessageBoxAsync(localizer, title, message);
    }

    /// <inheritdoc />
    public async Task ShowErrorMessageBoxAsync(Localizer localizer, string title, string message)
    {
        if (handler == null)
        {
            return;
        }
        await handler.ShowErrorMessageBoxAsync(localizer, title, message);
    }

    /// <inheritdoc />
    public async Task ShowErrorMessageBoxAsync(Localizer localizer, string title, MarkupString message)
    {
        if (handler == null)
        {
            return;
        }
        await handler.ShowErrorMessageBoxAsync(localizer, title, message);
    }

    /// <inheritdoc />
    public async Task ShowErrorMessageBoxAsync(Localizer localizer, string title, Exception exception)
    {
        if (handler == null)
        {
            return;
        }
        await handler.ShowErrorMessageBoxAsync(localizer, title, exception);
    }
}