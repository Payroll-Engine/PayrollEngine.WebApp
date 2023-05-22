using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace PayrollEngine.WebApp.Presentation
{
    public class UserNotificationService : IUserNotificationService
    {
        private IUserNotificationService handler;
        public IUserNotificationService Handler
        {
            get
            {
                if (handler == null)
                {
                    throw new InvalidOperationException("Please initialize the user notification handler");
                }
                return handler;
            }
        }

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
        public async Task ShowAsync(string message) =>
            await Handler.ShowAsync(message);

        /// <inheritdoc />
        public async Task ShowInformationAsync(string message) =>
            await Handler.ShowInformationAsync(message);

        /// <inheritdoc />
        public async Task ShowWarningAsync(string message) =>
            await Handler.ShowWarningAsync(message);

        /// <inheritdoc />
        public async Task ShowSuccessAsync(string message) =>
            await Handler.ShowSuccessAsync(message);

        /// <inheritdoc />
        public async Task ShowErrorAsync(string message) =>
            await Handler.ShowErrorAsync(message);

        /// <inheritdoc />
        public async Task ShowErrorAsync(Exception exception, string message = null) =>
            await Handler.ShowErrorAsync(exception, message);

        /// <inheritdoc />
        public async Task<bool> ShowMessageBoxAsync(string title, string message,
            string yesText = null, string noText = null, string cancelText = null) =>
            await Handler.ShowMessageBoxAsync(title, message, yesText, noText, cancelText);

        /// <inheritdoc />
        public async Task<bool> ShowMessageBoxAsync(string title, MarkupString message,
            string yesText = null, string noText = null, string cancelText = null) =>
            await Handler.ShowMessageBoxAsync(title, message, yesText, noText, cancelText);

        /// <inheritdoc />
        public async Task<bool> ShowDeleteMessageBoxAsync(string title, string message) =>
            await Handler.ShowDeleteMessageBoxAsync(title, message);

        /// <inheritdoc />
        public async Task<bool> ShowDeleteMessageBoxAsync(string title, MarkupString message) =>
            await Handler.ShowDeleteMessageBoxAsync(title, message);

        /// <inheritdoc />
        public async Task ShowErrorMessageBoxAsync(string title, string message) =>
            await Handler.ShowErrorMessageBoxAsync(title, message);

        /// <inheritdoc />
        public async Task ShowErrorMessageBoxAsync(string title, MarkupString message) =>
            await Handler.ShowErrorMessageBoxAsync(title, message);

        /// <inheritdoc />
        public async Task ShowErrorMessageBoxAsync(string title, Exception exception) =>
            await Handler.ShowErrorMessageBoxAsync(title, exception);
    }
}
