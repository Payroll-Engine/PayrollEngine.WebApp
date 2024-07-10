using System;
using MudBlazor;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using PayrollEngine.Client;
using PayrollEngine.WebApp.Shared;

namespace PayrollEngine.WebApp.Presentation;

public static class DialogServiceExtensions
{
    public static async Task<bool> ShowMessageBoxAsync(this IDialogService dialogService,
        string title, string message, string yesText = "OK",
        string noText = null, string cancelText = null)
    {
        var result = await dialogService.ShowMessageBox(title, message, yesText, noText,
            cancelText);
        return result == true;
    }

    public static async Task<bool> ShowMessageBoxAsync(this IDialogService dialogService,
        string title, MarkupString message, string yesText = "OK",
        string noText = null, string cancelText = null)
    {
        var result = await dialogService.ShowMessageBox(title, message, yesText, noText,
            cancelText);
        return result == true;
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