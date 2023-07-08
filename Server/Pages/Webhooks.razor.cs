using Microsoft.AspNetCore.Components;
using MudBlazor;
using PayrollEngine.WebApp.Presentation;
using PayrollEngine.WebApp.Presentation.BackendService;
using PayrollEngine.WebApp.Server.Dialogs;
using PayrollEngine.WebApp.Server.Shared;
using PayrollEngine.WebApp.ViewModel;
using Task = System.Threading.Tasks.Task;

namespace PayrollEngine.WebApp.Server.Pages;

public partial class Webhooks
{
    [Inject]
    private WebhookBackendService WebhookBackendService { get; set; }

    protected override string GridId => GetTenantGridId(GridIdentifiers.Webhooks);
    protected override IBackendService<Webhook, Query> BackendService => WebhookBackendService;
    protected override ItemCollection<Webhook> Items { get; } = new();
    protected override string GetLocalizedItemName(bool plural) => 
        plural ? Localizer.Webhook.Webhooks : Localizer.Webhook.Webhook;

    public Webhooks() :
        base(WorkingItems.TenantChange)
    {
    }

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
