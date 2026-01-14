using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using PayrollEngine.Client;
using PayrollEngine.WebApp.Shared;

namespace PayrollEngine.WebApp.Presentation.Component;

/// <summary>
/// extension methods for <see cref="IDialogService"/>
/// </summary>
public static class DialogServiceExtensions
{
    /// <param name="dialogService">Dialog service</param>
    extension(IDialogService dialogService)
    {
        /// <summary>
        /// Show message box
        /// </summary>
        /// <param name="title">Dialog title</param>
        /// <param name="message">Dialog message</param>
        /// <param name="yesText">Yes-button text</param>
        /// <param name="noText">No-button text</param>
        /// <param name="cancelText">Cancel-button text</param>
        /// <param name="icon">Display icon</param>
        /// <param name="iconColor">Display icon color</param>
        /// <param name="submitColor">Submit button color</param>
        /// <param name="submitVariant">Submit variant</param>
        /// <returns>True for ok-button, otherwise false</returns>
        public async Task<bool> ShowMessageBoxAsync(string title, string message, string yesText = "OK",
            string noText = null, string cancelText = null,
            string icon = Icons.Material.Filled.Info,
            Color iconColor = Color.Info,
            Color submitColor = Color.Tertiary,
            Variant? submitVariant = null)
        {
            var options = new MessageBoxOptions
            {
                CancelText = cancelText,
                YesText = yesText,
                NoText = noText,
                Message = message
            };
            submitVariant ??= Globals.ButtonVariant;
            var parameters = new DialogParameters
            {
                { nameof(MessageBoxDialog.Options), options },
                { nameof(MessageBoxDialog.Icon), icon },
                { nameof(MessageBoxDialog.IconColor), iconColor },
                { nameof(MessageBoxDialog.SubmitColor), submitColor },
                { nameof(MessageBoxDialog.SubmitVariant), submitVariant }
            };
            var result = await (await dialogService.ShowAsync<MessageBoxDialog>(title, parameters)).Result;
            return result != null && !result.Canceled;
        }

        /// <summary>
        /// Show message box wit markup string
        /// </summary>
        /// <param name="title">Dialog title</param>
        /// <param name="message">Dialog message</param>
        /// <param name="yesText">Yes-button text</param>
        /// <param name="noText">No-button text</param>
        /// <param name="cancelText">Cancel-button text</param>
        /// <param name="icon">Display icon</param>
        /// <param name="iconColor">Display icon color</param>
        /// <param name="submitColor">Submit button color</param>
        /// <param name="submitVariant">Submit variant</param>
        /// <returns>True for ok-button, otherwise false</returns>
        public async Task<bool> ShowMessageBoxAsync(string title, MarkupString message, string yesText = "OK",
            string noText = null, string cancelText = null,
            string icon = Icons.Material.Filled.Info,
            Color iconColor = Color.Info,
            Color submitColor = Color.Tertiary,
            Variant? submitVariant = null)
        {
            var options = new MessageBoxOptions
            {
                CancelText = cancelText,
                YesText = yesText,
                NoText = noText,
                MarkupMessage = message
            };
            submitVariant ??= Globals.ButtonVariant;
            var parameters = new DialogParameters
            {
                { nameof(MessageBoxDialog.Options), options },
                { nameof(MessageBoxDialog.Icon), icon },
                { nameof(MessageBoxDialog.IconColor), iconColor },
                { nameof(MessageBoxDialog.SubmitColor), submitColor },
                { nameof(MessageBoxDialog.SubmitVariant), submitVariant }
            };
            var result = await (await dialogService.ShowAsync<MessageBoxDialog>(title, parameters)).Result;
            return result != null && !result.Canceled;
        }

        /// <summary>
        /// Show delete message box
        /// </summary>
        /// <param name="localizer">localizer</param>
        /// <param name="title">Dialog title</param>
        /// <param name="message">Dialog message</param>
        /// <returns>True for ok-button, otherwise false</returns>
        public async Task<bool> ShowDeleteMessageBoxAsync(Localizer localizer,
            string title, string message) =>
            await dialogService.ShowMessageBoxAsync(title, message,
                yesText: localizer.Dialog.Delete,
                cancelText: localizer.Dialog.Cancel,
                icon: Icons.Material.Filled.Delete,
                iconColor: Color.Error,
                submitColor: Color.Error,
                submitVariant: Globals.ButtonVariant);

        /// <summary>
        /// Show delete message box with markup text
        /// </summary>
        /// <param name="localizer">localizer</param>
        /// <param name="title">Dialog title</param>
        /// <param name="message">Dialog message</param>
        /// <returns>True for ok-button, otherwise false</returns>
        public async Task<bool> ShowDeleteMessageBoxAsync(Localizer localizer,
            string title, MarkupString message) =>
            await dialogService.ShowMessageBoxAsync(title, message,
                yesText: localizer.Dialog.Delete,
                cancelText: localizer.Dialog.Cancel,
                icon: Icons.Material.Filled.Delete,
                iconColor: Color.Error,
                submitColor: Color.Error,
                submitVariant: Globals.ButtonVariant);

        /// <summary>
        /// Show error message box
        /// </summary>
        /// <param name="localizer">localizer</param>
        /// <param name="title">Dialog title</param>
        /// <param name="message">Dialog message</param>
        public async Task ShowErrorMessageBoxAsync(Localizer localizer,
            string title, string message) =>
            await dialogService.ShowMessageBoxAsync(title, message,
                yesText: localizer.Dialog.Ok,
                cancelText: string.Empty,
                icon: Icons.Material.Filled.ErrorOutline,
                iconColor: Color.Error);

        /// <summary>
        /// Show error message box with markup text
        /// </summary>
        /// <param name="localizer">localizer</param>
        /// <param name="title">Dialog title</param>
        /// <param name="message">Dialog message</param>
        public async Task ShowErrorMessageBoxAsync(Localizer localizer,
            string title, MarkupString message) =>
            await dialogService.ShowMessageBoxAsync(title, message,
                yesText: localizer.Dialog.Ok,
                cancelText: string.Empty,
                icon: Icons.Material.Filled.ErrorOutline,
                iconColor: Color.Error);

        /// <summary>
        /// Show error message box from exception
        /// </summary>
        /// <param name="localizer">localizer</param>
        /// <param name="title">Dialog title</param>
        /// <param name="exception">Exception</param>
        public async Task ShowErrorMessageBoxAsync(Localizer localizer,
            string title, Exception exception)
        {
            var message = new MarkupString(exception.GetApiErrorMessage()
                .Replace("\r\n", "<br />")
                .Replace(". ", ".<br />")
                .EnsureEnd("."));
            await dialogService.ShowErrorMessageBoxAsync(localizer, title, message);
        }
    }
}