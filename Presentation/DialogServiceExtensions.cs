using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using PayrollEngine.Client;
using PayrollEngine.WebApp.Shared;
using PayrollEngine.WebApp.Presentation.Component;

namespace PayrollEngine.WebApp.Presentation;

/// <summary>
/// extension methods for <see cref="IDialogService"/>
/// </summary>
public static class DialogServiceExtensions
{
    /// <summary>
    /// Show message box
    /// </summary>
    /// <param name="dialogService">Dialog service</param>
    /// <param name="title">Dialog title</param>
    /// <param name="message">Dialog message</param>
    /// <param name="yesText">Yes-button text</param>
    /// <param name="noText">No-button text</param>
    /// <param name="cancelText">Cancel-button text</param>
    /// <returns>True for ok-button, otherwise false</returns>
    public static async Task<bool> ShowMessageBoxAsync(this IDialogService dialogService,
        string title, string message, string yesText = "OK",
        string noText = null, string cancelText = null)
    {
        var options = new MessageBoxOptions
        {
            CancelText = cancelText,
            YesText = yesText,
            NoText = noText,
            Message = message
        };
        var parameters = new DialogParameters { { nameof(MessageBoxDialog.Options), options } };
        var result = await (await dialogService.ShowAsync<MessageBoxDialog>(title, parameters)).Result;
        return result != null && !result.Canceled;
    }

    /// <summary>
    /// Show message box wit markup string
    /// </summary>
    /// <param name="dialogService">Dialog service</param>
    /// <param name="title">Dialog title</param>
    /// <param name="message">Dialog message</param>
    /// <param name="yesText">Yes-button text</param>
    /// <param name="noText">No-button text</param>
    /// <param name="cancelText">Cancel-button text</param>
    /// <returns>True for ok-button, otherwise false</returns>
    public static async Task<bool> ShowMessageBoxAsync(this IDialogService dialogService,
        string title, MarkupString message, string yesText = "OK",
        string noText = null, string cancelText = null)
    {
        var options = new MessageBoxOptions
        {
            CancelText = cancelText,
            YesText = yesText,
            NoText = noText,
            MarkupMessage = message
        };
        var parameters = new DialogParameters { { nameof(MessageBoxDialog.Options), options } };
        var result = await (await dialogService.ShowAsync<MessageBoxDialog>(title, parameters)).Result;
        return result != null && !result.Canceled;
    }

    /// <summary>
    /// Show delete message box
    /// </summary>
    /// <param name="dialogService">Dialog service</param>
    /// <param name="localizer">localizer</param>
    /// <param name="title">Dialog title</param>
    /// <param name="message">Dialog message</param>
    /// <returns>True for ok-button, otherwise false</returns>
    public static async Task<bool> ShowDeleteMessageBoxAsync(this IDialogService dialogService, Localizer localizer,
        string title, string message) =>
        await ShowMessageBoxAsync(dialogService, title, message,
            yesText: localizer.Dialog.Delete,
            cancelText: localizer.Dialog.Cancel);

    /// <summary>
    /// Show delete message box with markup text
    /// </summary>
    /// <param name="dialogService">Dialog service</param>
    /// <param name="localizer">localizer</param>
    /// <param name="title">Dialog title</param>
    /// <param name="message">Dialog message</param>
    /// <returns>True for ok-button, otherwise false</returns>
    public static async Task<bool> ShowDeleteMessageBoxAsync(this IDialogService dialogService, Localizer localizer,
        string title, MarkupString message) =>
        await ShowMessageBoxAsync(dialogService, title, message,
            yesText: localizer.Dialog.Delete,
            cancelText: localizer.Dialog.Cancel);

    /// <summary>
    /// Show error message box
    /// </summary>
    /// <param name="dialogService">Dialog service</param>
    /// <param name="localizer">localizer</param>
    /// <param name="title">Dialog title</param>
    /// <param name="message">Dialog message</param>
    public static async Task ShowErrorMessageBoxAsync(this IDialogService dialogService, Localizer localizer,
        string title, string message) =>
        await ShowMessageBoxAsync(dialogService, title, message,
            yesText: localizer.Dialog.Ok,
            cancelText: string.Empty);

    /// <summary>
    /// Show error message box with markup text
    /// </summary>
    /// <param name="dialogService">Dialog service</param>
    /// <param name="localizer">localizer</param>
    /// <param name="title">Dialog title</param>
    /// <param name="message">Dialog message</param>
    public static async Task ShowErrorMessageBoxAsync(this IDialogService dialogService, Localizer localizer,
        string title, MarkupString message) =>
        await ShowMessageBoxAsync(dialogService, title, message,
            yesText: localizer.Dialog.Ok,
            cancelText: string.Empty);

    /// <summary>
    /// Show error message box from exception
    /// </summary>
    /// <param name="dialogService">Dialog service</param>
    /// <param name="localizer">localizer</param>
    /// <param name="title">Dialog title</param>
    /// <param name="exception">Exception</param>
    public static async Task ShowErrorMessageBoxAsync(this IDialogService dialogService, Localizer localizer,
        string title, Exception exception)
    {
        var message = new MarkupString(exception.GetApiErrorMessage()
            .Replace("\r\n", "<br />")
            .Replace(". ", ".<br />")
            .EnsureEnd("."));
        await ShowErrorMessageBoxAsync(dialogService, localizer, title, message);
    }
}