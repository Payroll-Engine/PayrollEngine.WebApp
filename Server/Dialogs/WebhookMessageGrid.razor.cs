using System;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using PayrollEngine.Client.Service;
using PayrollEngine.WebApp.Presentation;
using PayrollEngine.WebApp.ViewModel;
using Task = System.Threading.Tasks.Task;

namespace PayrollEngine.WebApp.Server.Dialogs;

public partial class WebhookMessageGrid : IDisposable
{
    [Parameter]
    public Tenant Tenant { get; set; }
    [Parameter]
    public Webhook Webhook { get; set; }
    [Parameter]
    public string Class { get; set; }
    [Parameter]
    public string Style { get; set; }
    [Parameter]
    public string Height { get; set; }

    [Inject]
    private IWebhookMessageService WebhookMessageService { get; set; }
    [Inject]
    private IDialogService DialogService { get; set; }
    [Inject]
    private IUserNotificationService UserNotification { get; set; }

    protected ItemCollection<WebhookMessage> WebhookMessages { get; set; } = new();
    protected MudDataGrid<WebhookMessage> Grid { get; set; }

    #region Actions

    protected async Task AddWebhookMessageAsync()
    {
        // webhook message create dialog
        var dialog = await (await DialogService.ShowAsync<WebhookMessageDialog>("Add webhook message")).Result;
        if (dialog == null || dialog.Canceled)
        {
            return;
        }

        // new webhook message
        if (dialog.Data is not WebhookMessage webhookMessage)
        {
            return;
        }

        // add webhook message
        try
        {
            var result = await WebhookMessageService.CreateAsync(new(Tenant.Id, Webhook.Id), webhookMessage);
            if (result != null)
            {
                WebhookMessages.Add(webhookMessage);
                await UserNotification.ShowSuccessAsync($"Webhook message {webhookMessage.ActionName} added");
            }
        }
        catch (Exception exception)
        {
            Log.Error(exception, exception.GetBaseMessage());
            await UserNotification.ShowErrorMessageBoxAsync("Add webhook message", exception);
        }
    }

    protected async Task EditWebhookMessageAsync(WebhookMessage webhookMessage)
    {
        // existing
        if (!WebhookMessages.Contains(webhookMessage))
        {
            return;
        }

        // edit copy
        var editItem = new WebhookMessage(webhookMessage);

        // dialog parameters
        var parameters = new DialogParameters
        {
            { nameof(WebhookMessageDialog.WebhookMessage), editItem }
        };

        // webhook message edit dialog
        var dialog = await (await DialogService.ShowAsync<WebhookMessageDialog>("Edit webhook message", parameters)).Result;
        if (dialog == null || dialog.Canceled)
        {
            return;
        }

        // replace webhook message
        try
        {
            await WebhookMessageService.UpdateAsync(new(Tenant.Id, Webhook.Id), editItem);
            WebhookMessages.Remove(webhookMessage);
            WebhookMessages.Add(editItem);
            await UserNotification.ShowSuccessAsync($"Webhook message {webhookMessage.ActionName} updated");
        }
        catch (Exception exception)
        {
            Log.Error(exception, exception.GetBaseMessage());
            await UserNotification.ShowErrorMessageBoxAsync("Edit webhook message", exception);
        }
    }

    protected async Task DeleteWebhookMessageAsync(WebhookMessage webhookMessage)
    {
        // existing
        if (!WebhookMessages.Contains(webhookMessage))
        {
            return;
        }

        // confirmation
        if (!await DialogService.ShowDeleteMessageBoxAsync(
                "Delete webhook message",
                $"Delete {webhookMessage.ActionName} permanently?"))
        {
            return;
        }

        // delete webhook message
        try
        {
            await WebhookMessageService.DeleteAsync(new(Tenant.Id, Webhook.Id), webhookMessage.Id);
            WebhookMessages.Remove(webhookMessage);
            await UserNotification.ShowSuccessAsync($"Webhook message {webhookMessage.ActionName} removed");
        }
        catch (Exception exception)
        {
            Log.Error(exception, exception.GetBaseMessage());
            await UserNotification.ShowErrorMessageBoxAsync("Delete webhook message", exception);
        }
    }

    #endregion

    #region Lifecycle

    private async Task SetupWebhookMessages()
    {
        WebhookMessages.Clear();

        // read webhook message
        try
        {
            var parameters = await WebhookMessageService.QueryAsync<WebhookMessage>(new(Tenant.Id, Webhook.Id),
                new() { Status = ObjectStatus.Active });
            foreach (var parameter in parameters)
            {
                WebhookMessages.Add(new(parameter));
            }
        }
        catch (Exception exception)
        {
            Log.Error(exception, exception.GetBaseMessage());
            await UserNotification.ShowErrorMessageBoxAsync("Delete webhook message", exception);
        }
    }

    protected override async Task OnInitializedAsync()
    {
        await Task.Run(SetupWebhookMessages);
        await base.OnInitializedAsync();
    }

    public void Dispose()
    {
        WebhookMessages?.Dispose();
    }

    #endregion

}
