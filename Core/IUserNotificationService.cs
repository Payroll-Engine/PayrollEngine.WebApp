using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using PayrollEngine.WebApp.Shared;

namespace PayrollEngine.WebApp;

public interface IUserNotificationService
{
    /// <summary>
    /// The notification handler
    /// </summary>
    IUserNotificationService Handler { get; }

    /// <summary>
    /// Initialize the notification handler
    /// </summary>
    /// <param name="handler">The notification handler</param>
    void Initialize(IUserNotificationService handler);

    /// <summary>
    /// Test if service is initialized
    /// </summary>
    bool IsInitialized { get; }

    /// <summary>
    /// Show a message
    /// </summary>
    /// <param name="message">The message to show</param>
    Task ShowAsync(string message);

    /// <summary>
    /// Show an information message
    /// </summary>
    /// <param name="message">The message to show</param>
    Task ShowInformationAsync(string message);

    /// <summary>
    /// Show a warning message
    /// </summary>
    /// <param name="message">The message to show</param>
    Task ShowWarningAsync(string message);

    /// <summary>
    /// Show a success message
    /// </summary>
    /// <param name="message">The message to show</param>
    Task ShowSuccessAsync(string message);

    /// <summary>
    /// Show an error message
    /// </summary>
    /// <param name="message">The message to show</param>
    Task ShowErrorAsync(string message);

    /// <summary>
    /// Show an exception error message
    /// </summary>
    /// <param name="exception">The exception</param>
    /// <param name="message">The message to show</param>
    Task ShowErrorAsync(Exception exception, string message = null);

    /// <summary>
    /// Show a message box
    /// </summary>
    /// <param name="localizer">The localizer</param>
    /// <param name="title">The dialog title</param>
    /// <param name="message">The message to show</param>
    /// <param name="yesText">The yes button text</param>
    /// <param name="noText">The no button text</param>
    /// <param name="cancelText">The cancel button text</param>
    Task<bool> ShowMessageBoxAsync(Localizer localizer, string title, string message, string yesText = null,
        string noText = null, string cancelText = null);

    /// <summary>
    /// Show a message box
    /// </summary>
    /// <param name="localizer">The localizer</param>
    /// <param name="title">The dialog title</param>
    /// <param name="message">The message to show</param>
    /// <param name="yesText">The yes button text</param>
    /// <param name="noText">The no button text</param>
    /// <param name="cancelText">The cancel button text</param>
    Task<bool> ShowMessageBoxAsync(Localizer localizer, string title, MarkupString message, string yesText = null,
        string noText = null, string cancelText = null);

    /// <summary>
    /// Show a delete confirmation message box
    /// </summary>
    /// <param name="localizer">The localizer</param>
    /// <param name="title">The dialog title</param>
    /// <param name="message">The message to show</param>
    Task<bool> ShowDeleteMessageBoxAsync(Localizer localizer, string title, string message);

    /// <summary>
    /// Show a delete confirmation message box
    /// </summary>
    /// <param name="localizer">The localizer</param>
    /// <param name="title">The dialog title</param>
    /// <param name="message">The message to show</param>
    Task<bool> ShowDeleteMessageBoxAsync(Localizer localizer, string title, MarkupString message);

    /// <summary>
    /// Show an error message box
    /// </summary>
    /// <param name="localizer">The localizer</param>
    /// <param name="title">The dialog title</param>
    /// <param name="message">The message to show</param>
    Task ShowErrorMessageBoxAsync(Localizer localizer, string title, string message);

    /// <summary>
    /// Show an error message box
    /// </summary>
    /// <param name="localizer">The localizer</param>
    /// <param name="title">The dialog title</param>
    /// <param name="message">The message to show</param>
    Task ShowErrorMessageBoxAsync(Localizer localizer, string title, MarkupString message);

    /// <summary>
    /// Show an error exception message box
    /// </summary>
    /// <param name="localizer">The localizer</param>
    /// <param name="title">The dialog title</param>
    /// <param name="exception">The exception to show</param>
    Task ShowErrorMessageBoxAsync(Localizer localizer, string title, Exception exception);
}