using Microsoft.AspNetCore.Components;
using MudBlazor;
using PayrollEngine.WebApp.Presentation;
using PayrollEngine.WebApp.Presentation.BackendService;
using PayrollEngine.WebApp.Presentation.Component;
using PayrollEngine.WebApp.Server.Components.Dialogs;
using PayrollEngine.WebApp.Server.Components.Shared;
using PayrollEngine.WebApp.ViewModel;
using Task = System.Threading.Tasks.Task;

namespace PayrollEngine.WebApp.Server.Components.Pages;

public partial class Webhooks() : EditItemPageBase<Webhook, Query, WebhookDialog>(WorkingItems.TenantChange) 
{
    [Inject]
    private WebhookBackendService WebhookBackendService { get; set; }

    protected override string GridId => GetTenantGridId(GridIdentifiers.Webhooks);
    protected override IBackendService<Webhook, Query> BackendService => WebhookBackendService;
    protected override ItemCollection<Webhook> Items { get; } = new();
    protected override string GetLocalizedItemName(bool plural) => 
        plural ? Localizer.Webhook.Webhooks : Localizer.Webhook.Webhook;

    private async Task WebhookMessagesAsync(Webhook webhook)
    {
        if (Session.User == null)
        {
            return;
        }

        // report parameters
        var parameters = new DialogParameters
        {
            { nameof(WebhookMessagesDialog.Tenant), Session.Tenant },
            { nameof(WebhookMessagesDialog.Webhook), webhook }
        };
        await DialogService.ShowAsync<WebhookMessagesDialog>("Webhook Messages", parameters);
    }
}
