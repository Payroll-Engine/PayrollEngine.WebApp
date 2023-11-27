﻿using System.Collections.Generic;
using System.Net.Http;
using PayrollEngine.Client;
using PayrollEngine.Client.Service;
using PayrollEngine.Client.Service.Api;
using PayrollEngine.WebApp.Shared;

namespace PayrollEngine.WebApp.Presentation.BackendService;

public class CalendarBackendService(UserSession userSession, HttpClientHandler httpClientHandler,
        PayrollHttpConfiguration configuration, Localizer localizer)
    : BackendServiceBase<CalendarService, TenantServiceContext, ViewModel.Calendar, Query>(userSession, httpClientHandler, configuration, localizer)
{
    /// <summary>The current request context</summary>
    protected override TenantServiceContext CreateServiceContext(IDictionary<string, object> parameters = null) =>
        UserSession.Tenant != null ? new TenantServiceContext(UserSession.Tenant.Id) : null;

    /// <summary>Create the backend service</summary>
    protected override CalendarService CreateService() =>
        new(HttpClient);
}