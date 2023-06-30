using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using PayrollEngine.WebApp.Shared;

namespace PayrollEngine.WebApp.Presentation.Component;

public class UserNotificationBase : ComponentBase, IUserNotificationService
{
    [Inject]
    private ISnackbar Snackbar { get; set; }
    [Inject]
    private IDialogService DialogService { get; set; }

    /// <inheritdoc />
    void IUserNotificationService.Initialize(IUserNotificationService handler) =>
        throw new NotSupportedException();

    /// <inheritdoc />
    public bool IsInitialized => false;

    /// <inheritdoc />
    public async Task ShowAsync(string message) =>
        await ShowToastAsync(message, Severity.Normal);

    /// <inheritdoc />
    public async Task ShowInformationAsync(string message) =>
        await ShowToastAsync(message, Severity.Info);

    /// <inheritdoc />
    public async Task ShowWarningAsync(string message) =>
        await ShowToastAsync(message, Severity.Warning);

    /// <inheritdoc />
    public async Task ShowSuccessAsync(string message) =>
        await ShowToastAsync(message, Severity.Success);

    /// <inheritdoc />
    public async Task ShowErrorAsync(string message) =>
        await ShowToastAsync(message, Severity.Error);

    /// <inheritdoc />
    public async Task ShowErrorAsync(Exception exception, string message = null) =>
        await ShowToastAsync(message ?? exception?.GetBaseMessage(), Severity.Error);

    /// <inheritdoc />
    public async Task<bool> ShowMessageBoxAsync(Localizer localizer, string title, string message,
        string yesText = null, string noText = null, string cancelText = null) =>
        await DialogService.ShowMessageBoxAsync(title, message, yesText ?? localizer.Dialog.Ok, noText, cancelText);

    /// <inheritdoc />
    public async Task<bool> ShowMessageBoxAsync(Localizer localizer, string title, MarkupString message,
        string yesText = null, string noText = null, string cancelText = null) =>
        await DialogService.ShowMessageBoxAsync(title, message, yesText ?? localizer.Dialog.Ok, noText, cancelText);

    /// <inheritdoc />
    public async Task<bool> ShowDeleteMessageBoxAsync(Localizer localizer, string title, string message) =>
        await DialogService.ShowDeleteMessageBoxAsync(localizer, title, message);

    /// <inheritdoc />
    public async Task<bool> ShowDeleteMessageBoxAsync(Localizer localizer, string title, MarkupString message) =>
        await DialogService.ShowDeleteMessageBoxAsync(localizer, title, message);

    /// <inheritdoc />
    public async Task ShowErrorMessageBoxAsync(Localizer localizer, string title, string message) =>
        await DialogService.ShowErrorMessageBoxAsync(localizer, title, message);

    /// <inheritdoc />
    public async Task ShowErrorMessageBoxAsync(Localizer localizer, string title, MarkupString message) =>
        await DialogService.ShowErrorMessageBoxAsync(localizer, title, message);

    /// <inheritdoc />
    public async Task ShowErrorMessageBoxAsync(Localizer localizer, string title, Exception exception)
    {
        await DialogService.ShowErrorMessageBoxAsync(localizer, title, exception);
    }

    private Task ShowToastAsync(string message, Severity severity)
    {
        Snackbar.Add(message, severity);
        return Task.CompletedTask;
    }
}