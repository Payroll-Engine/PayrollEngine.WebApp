using System.Collections.Generic;
using PayrollEngine.Client;
using PayrollEngine.Client.Service;
using PayrollEngine.Client.Service.Api;
using PayrollEngine.WebApp.Shared;
using PayrollEngine.WebApp.ViewModel;

namespace PayrollEngine.WebApp.Presentation.BackendService;

public class WebhookMessageBackendService(UserSession userSession,
    PayrollHttpClient httpClient,
    ILocalizerService localizerService)
    : BackendServiceBase<WebhookMessageService, WebhookServiceContext, WebhookMessage, Query>(
        userSession, httpClient, localizerService)
{
    /// <summary>The current request context</summary>
    protected override WebhookServiceContext CreateServiceContext(IDictionary<string, object> parameters = null)
    {
        var parentWebhook = parameters?.GetValue<IRegulationItem>(nameof(IRegulationItem.Parent));
        if (parentWebhook == null)
        {
            throw new PayrollException("Missing webhook for message.");
        }
        if (UserSession.Tenant != null && UserSession.Payroll != null)
        {
            return new(UserSession.Tenant.Id, parentWebhook.Id);
        }
        return null;
    }

    /// <summary>Create the backend service</summary>
    protected override WebhookMessageService CreateService() =>
        new(HttpClient);
}