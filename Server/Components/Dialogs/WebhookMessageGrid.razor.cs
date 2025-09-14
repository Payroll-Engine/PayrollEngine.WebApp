using System;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using PayrollEngine.Client.Service;
using PayrollEngine.WebApp.Presentation.Component;
using PayrollEngine.WebApp.Shared;
using PayrollEngine.WebApp.ViewModel;
using Task = System.Threading.Tasks.Task;

namespace PayrollEngine.WebApp.Server.Components.Dialogs;

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
    [Inject]
    private ILocalizerService LocalizerService { get; set; }

    private Localizer Localizer => LocalizerService.Localizer;
    private ItemCollection<WebhookMessage> WebhookMessages { get; } = new();
    // ReSharper disable once UnusedAutoPropertyAccessor.Local
    private MudDataGrid<WebhookMessage> Grid { get; set; }

    #region Actions

    private async Task CreateWebhookMessageAsync()
    {
        // webhook message create dialog
        var dialog = await (await DialogService.ShowAsync<WebhookMessageDialog>(
            Localizer.Item.CreateTitle(Localizer.WebhookMessage.WebhookMessage))).Result;
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
                WebhookMessages.Add(result);
                await UserNotification.ShowSuccessAsync(Localizer.Item.Created(webhookMessage.ActionName));
            }
        }
        catch (Exception exception)
        {
            Log.Error(exception, exception.GetBaseMessage());
            await UserNotification.ShowErrorMessageBoxAsync(Localizer,
                Localizer.Item.CreateTitle(Localizer.WebhookMessage.WebhookMessage), exception);
        }
    }

    private async Task DeleteWebhookMessageAsync(WebhookMessage webhookMessage)
    {
        // existing
        if (!WebhookMessages.Contains(webhookMessage))
        {
            return;
        }

        // confirmation
        if (!await DialogService.ShowDeleteMessageBoxAsync(
                Localizer,
                Localizer.Item.DeleteTitle(Localizer.WebhookMessage.WebhookMessage),
                Localizer.Item.DeleteQuery(Localizer.WebhookMessage.WebhookMessage)))
        {
            return;
        }

        // delete webhook message
        try
        {
            // backend
            await WebhookMessageService.DeleteAsync(new(Tenant.Id, Webhook.Id), webhookMessage.Id);
            // client
            WebhookMessages.Remove(webhookMessage);
            await UserNotification.ShowSuccessAsync(Localizer.Item.Removed(webhookMessage.ActionName));
        }
        catch (Exception exception)
        {
            Log.Error(exception, exception.GetBaseMessage());
            await UserNotification.ShowErrorMessageBoxAsync(Localizer,
                Localizer.Item.DeleteTitle(Localizer.WebhookMessage.WebhookMessage), exception);
        }
    }

    #endregion

    #region Lifecycle

    protected override async Task OnInitializedAsync()
    {
        await SetupWebhookMessagesAsync();
        await base.OnInitializedAsync();
    }

    public void Dispose()
    {
        WebhookMessages?.Dispose();
    }

    private async Task SetupWebhookMessagesAsync()
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
            await UserNotification.ShowErrorMessageBoxAsync(
                Localizer,
                Localizer.WebhookMessage.WebhookMessage, exception);
        }
    }

    #endregion

}
