using System;
using MudBlazor;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using PayrollEngine.Client;

namespace PayrollEngine.WebApp.Presentation;

public static class DialogServiceExtensions
{
    public static async Task<bool> ShowMessageBoxAsync(this IDialogService dialogService,
        string title, string message, string yesText = null,
        string noText = null, string cancelText = null)
    {
        var result = await dialogService.ShowMessageBox(title, message, yesText, noText,
            cancelText);
        return result == true;
    }

    public static async Task<bool> ShowMessageBoxAsync(this IDialogService dialogService,
        string title, MarkupString message, string yesText = null,
        string noText = null, string cancelText = null)
    {
        var result = await dialogService.ShowMessageBox(title, message, yesText, noText,
            cancelText);
        return result == true;
    }

    public static async Task<bool> ShowDeleteMessageBoxAsync(this IDialogService dialogService,
        string title, string message) =>
        await ShowMessageBoxAsync(dialogService, title, message, yesText: "Delete", cancelText: "Cancel");

    public static async Task<bool> ShowDeleteMessageBoxAsync(this IDialogService dialogService,
        string title, MarkupString message) =>
        await ShowMessageBoxAsync(dialogService, title, message, yesText: "Delete", cancelText: "Cancel");

    public static async Task ShowErrorMessageBoxAsync(this IDialogService dialogService,
        string title, string message) =>
        await ShowMessageBoxAsync(dialogService, title, message, yesText: "OK", cancelText: string.Empty);

    public static async Task ShowErrorMessageBoxAsync(this IDialogService dialogService,
        string title, MarkupString message) =>
        await ShowMessageBoxAsync(dialogService, title, message, yesText: "OK", cancelText: string.Empty);

    public static async Task ShowErrorMessageBoxAsync(this IDialogService dialogService,
        string title, Exception exception)
    {
        var message = new MarkupString(exception.GetApiErrorMessage()
            .Replace("\r\n", "<br />")
            .Replace(". ", ".<br />")
            .EnsureEnd("."));
        await ShowErrorMessageBoxAsync(dialogService, title, message);
    }
}