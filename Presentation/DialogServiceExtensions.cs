using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using PayrollEngine.Client;
using PayrollEngine.WebApp.Shared;
using PayrollEngine.WebApp.Presentation.Component;

namespace PayrollEngine.WebApp.Presentation;

public static class DialogServiceExtensions
{
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

    public static async Task<bool> ShowDeleteMessageBoxAsync(this IDialogService dialogService, Localizer localizer,
        string title, string message) =>
        await ShowMessageBoxAsync(dialogService, title, message, 
            yesText: localizer.Dialog.Delete, 
            cancelText: localizer.Dialog.Cancel);

    public static async Task<bool> ShowDeleteMessageBoxAsync(this IDialogService dialogService, Localizer localizer,
        string title, MarkupString message) =>
        await ShowMessageBoxAsync(dialogService, title, message,
            yesText: localizer.Dialog.Delete, 
            cancelText: localizer.Dialog.Cancel);

    public static async Task ShowErrorMessageBoxAsync(this IDialogService dialogService, Localizer localizer,
        string title, string message) =>
        await ShowMessageBoxAsync(dialogService, title, message,
            yesText: localizer.Dialog.Ok, 
            cancelText: string.Empty);

    public static async Task ShowErrorMessageBoxAsync(this IDialogService dialogService, Localizer localizer,
        string title, MarkupString message) =>
        await ShowMessageBoxAsync(dialogService, title, message, 
            yesText: localizer.Dialog.Ok, 
            cancelText: string.Empty);

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